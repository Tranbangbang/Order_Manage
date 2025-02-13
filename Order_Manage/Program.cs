using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Order_Manage.Models;
using Order_Manage.XML;
using Order_Manage.Repository;
using Order_Manage.Repository.Impl;
using Order_Manage.Service;
using Order_Manage.Service.Impl;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Order_Manage.Common.Constants.Helper;
using Order_Manage.Common.Hubs;
using Fleck;
using Order_Manage.Kafka;
using Order_Manage.Kafka.Dto;
using Order_Manage.Kafka.Impl;


var builder = WebApplication.CreateBuilder(args);

#region.ConnectMysql
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
#endregion

#region.Auth
builder.Services.AddIdentity<Account, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 6;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"] ?? "DefaultSuperSecretKey";
#endregion

builder.Services.AddControllers();
builder.Services.AddCustomizeSwagger();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DapperContext>();
//builder.Services.AddSingleton<WebSocketHandler>();

builder.Services.Configure<KafkaConfiguration>(builder.Configuration.GetSection("KafkaConfiguration"));
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddHostedService<KafkaConsumerService>();


#region.Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IOrderService, OrderServiceImpl>();
builder.Services.AddScoped<INotificationService, NotificationServiceImpl>();
builder.Services.AddScoped<IQrCodeService, QrCodeServiceImpl>();
#endregion
#region.Repository
builder.Services.AddScoped<IAuthRepository, AuthRepositoryImpl>();
builder.Services.AddScoped<IAccountRepository, AccountRepositoryImpl>();
builder.Services.AddScoped<IOrderRepository, OrderRepositoryImpl>();
builder.Services.AddScoped<INotificationRepository, NotificationRepositoryImpl>();
builder.Services.AddScoped<IMessageRepository, MessageRepositoryImpl>();
builder.Services.AddScoped<WebSocketHandler>();

#endregion
#region.cookie
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});
#endregion
#region.Session
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion
#region.Google
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; 
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) 
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = new PathString("/api/oauth/google");
});


#endregion

builder.Services.AddSignalR();

string filePath = Path.Combine("XML", "SQL.xml");
builder.Services.AddSingleton<QueryLoader>(_ => new QueryLoader(filePath));


var app = builder.Build();
app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeRoles(services);
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    await SeedData.InitializeAdminAccount(services, configuration);
}
app.MapHub<NotificationHub>("/notificationHub");
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var userIdString = context.Request.Query["userId"];
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                context.Response.StatusCode = 400; // Bad Request
                await context.Response.WriteAsync("Invalid or missing userId");
                return;
            }

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
            await webSocketHandler.Handle(context, webSocket, userId.ToString());
        }
        else
        {
            context.Response.StatusCode = 400; // Not a WebSocket request
        }
    }
    else
    {
        await next();
    }
});

app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
