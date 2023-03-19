using AutoMapper;
using Contracts.Identity;
using Entities;
using Entities.Identity.AuthenticateRequestModels;
using Entities.Identity.DataTransferObject;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Auction.Controllers.IdentityController;

[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthenticateRepository _repository;
    private readonly ILogger<AuthenticateController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    //Method GetToken when login
    public AuthenticateController(ILogger<AuthenticateController> logger, IAuthenticateRepository repository, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }
    

    [HttpGet("privacy")]
    [Authorize]
    public async Task<IActionResult> GetPrivacy()
    {
        try
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return await Task.FromResult<IActionResult>(Ok(claims));
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetPrivacy action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpGet("user-profile")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile()
    {
        try
        {
            var userName = _userManager.GetUserName(User);
            if (userName == null) return BadRequest("User not found.");
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var listRoleNames = await _userManager.GetRolesAsync(user);
            var listRoles = await _roleManager.Roles
                .Where(r => r.Name != null && listRoleNames.Contains(r.Name))
                .ToListAsync();
            user.Roles = listRoles;
            var userResult = _mapper.Map<UserDto>(user);
            return Ok(userResult);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetUserProfile action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpGet("emailConfirmation")]
    public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
    {
        try
        {
            await _repository.EmailConfirmation(email, token);
            _logger.LogDebug("Email confirmation successful");
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside EmailConfirmation action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (request is not { UserNameOrEmail: { }, Password: { } })
                return BadRequest(new Response()
                {
                    Title = "Error",
                    Message = "Username or Email is required"
                });
            var result = await _repository.LoginAsync(request);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to authenticate user. Error : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost("register-for-client")]
    public async Task<IActionResult> RegisterForClient([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid) throw new Exception("User information is not valid");
            var result = await _repository.RegisterForClient(request);
            _logger.LogDebug("User created successfully");
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside RegisterForClient action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest();
            await _repository.ForgotPassword(request);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside ForgotPassword action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest();
            await _repository.ResetPassword(request);
            _logger.LogDebug("User reset password successful");
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside ResetPassword action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}