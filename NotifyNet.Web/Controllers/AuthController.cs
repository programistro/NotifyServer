using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;
using NotifyNet.Core.Models;

namespace NotifyNet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;
    private readonly UserManager<Employee> _userManager;

    public AuthController(ILogger<AuthController> logger, IUserService userService,
        UserManager<Employee> userManager)
    {
        _logger = logger;
        _userService = userService;
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto dto)
    {
        var findUser = await _userManager.FindByNameAsync(dto.Email);

        if (findUser != null)
        {
            return BadRequest("user already exists");
        }
        
        var user = await _userService.AddAsync(dto);

        if (user == null)
        {
            return BadRequest(new { message = "Username or password is incorrect" });
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email), 
            new Claim(ClaimTypes.GivenName, user.Name), 
        };

        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserDto credentials)
    {
        if (!ModelState.IsValid || credentials == null)
        {
            return new BadRequestObjectResult(new { Message = "Login failed" });
        }

        var identityUser = await _userManager.FindByNameAsync(credentials.Email);
        if (identityUser == null)
        {
            return new BadRequestObjectResult(new { Message = "Login failed" });
        }

        var result = _userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, credentials.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return new BadRequestObjectResult(new { Message = "Login failed" });
        }

        var claims = new List<Claim>
            {
                new(ClaimTypes.Email, identityUser.Email),
                new(ClaimTypes.Name, identityUser.UserName)
            };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
        
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(15)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
    }
}
