using Logging_Service.Config;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        IndexFormat = "logs-{0:yyyy.MM.dd}",  // ??m b?o r?ng ??nh d?ng ch? m?c ch�nh x�c
        AutoRegisterTemplate = true, // ??m b?o template t? ??ng ???c ??ng k�
        ModifyConnectionSettings = x => x.BasicAuthentication("elastic", "Cs_8EaS9wjqJDv9TpR7Z") // (N?u c� x�c th?c) // ??m b?o template t? ??ng ???c ??ng k�
    })
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();  // S? d?ng Serilog cho to�n b? ?ng d?ng

// C?u h�nh d?ch v? cho ?ng d?ng
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ElasticsearchSettings>(builder.Configuration.GetSection("ElasticsearchSettings"));

var app = builder.Build();

// C?u h�nh HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// C?u h�nh CORS ?? cho ph�p c�c microservices kh�c g?i log ??n
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();
app.Run();
