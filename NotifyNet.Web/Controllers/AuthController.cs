using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AXO.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;

namespace NotifyNet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;
    private readonly SignInManager<Employee> _signInManager;

    public AuthController(ILogger<AuthController> logger, IUserService userService, SignInManager<Employee> signInManager)
    {
        _logger = logger;
        _userService = userService;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto dto)
    {
        var findUser = await _userService.GetByEmailAsync(dto.Email);

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

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserDto dto)
    {
        var user = await _userService.GetByEmailAsync(dto.Email);
        
        if(user == null)
            return NotFound("user not found");

        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, true, false).ConfigureAwait(false);

        if (result.Succeeded)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Email) };
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));   
        }
        else
        {
            return BadRequest(new { message = "Username or password is incorrect" });
        }
    }
}
