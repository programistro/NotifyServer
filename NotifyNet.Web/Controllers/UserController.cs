using Microsoft.AspNetCore.Mvc;
using NotifyNet.Application.Interface;
using NotifyNet.Core.Dto;
using NotifyNet.Core.Models;

namespace NotifyNet.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpGet("get-user-by-email")]
    public async Task<IActionResult> GetUser(string email)
    {
        var user = await _userService.GetByEmailAsync(email);

        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }

    [HttpGet("get-user-by-id")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }

    [HttpGet("get-user-by-name")]
    public async Task<IActionResult> GetUserByName(string name)
    {
        var user = await _userService.GetByNameAsync(name);

        if (user == null)
        {
            return NotFound();
        }
        
        return Ok(user);
    }

    [HttpPut("update-user-by-id")]
    public async Task<IActionResult> UpdateById(Employee employee)
    {
        var user = await _userService.GetByIdAsync(employee.Id);

        if (user == null)
        {
            return NotFound();
        }

        await _userService.Update(user);
        
        return Ok(user);
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(UserDto dto)
    {
        var findUser = await _userService.GetByEmailAsync(dto.Email);

        if (findUser != null)
        {
            return BadRequest("user already exists");
        }
        
        var user = await _userService.AddAsync(dto);

        return Ok(user);
    }

    [HttpDelete("delete-user-by-id")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        await _userService.Delete(id);
        
        return Ok("user was deleted");
    }

    [HttpDelete("delete-user-by-email")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        var user = await _userService.GetByEmailAsync(email);

        if (user == null)
        {
            return NotFound();
        }

        await _userService.Delete(user.Id);
        
        return Ok("user was deleted");
    }
}