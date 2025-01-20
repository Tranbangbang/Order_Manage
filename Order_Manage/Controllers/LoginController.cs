using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Order_Manage.Dto.Helper;
using Order_Manage.Dto.Request;
using Order_Manage.Service;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Order_Manage.Models;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _accountService;
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly IConfiguration _configuration;

    public LoginController(ILoginService accountService,UserManager<Account> userManager, SignInManager<Account> signInManager, IConfiguration configuration)
    {
        _accountService = accountService;
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("create")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = $"{UserRoles.Admin}")]
    public async Task<IActionResult> Create([FromBody] RegisterRequest request)
    {
        var response = await _accountService.Create(request);
        return StatusCode(response.Code, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _accountService.Login(request);
        return StatusCode(response.Code, response);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SignupRequest request)
    {
        var response = await _accountService.Register(request);
        return StatusCode(response.Code, response);
    }


    [HttpGet("login-google")]
    public IActionResult LoginWithGoogle()
    {
        var redirectUrl = "https://localhost:7220/api/oauth/google";
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }


   
}
