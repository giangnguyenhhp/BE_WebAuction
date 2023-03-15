using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Entities;
using Entities.Identity.AuthenticateRequestModels;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Web_Auction.Controllers.IdentityController;

[ApiController]
[Route("api/[controller]")]
public class AuthenticateController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<AuthenticateController> _logger;

    //Method GetToken when login
    public AuthenticateController(IConfiguration configuration, SignInManager<AppUser> signInManager,
        RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ILogger<AuthenticateController> logger)
    {
        _configuration = configuration;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            claims: authClaims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
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
            var checkUser = await _userManager.FindByNameAsync(request.UserNameOrEmail);
            if (checkUser == null)
            {
                checkUser = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
                if (checkUser == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response()
                    {
                        Title = "Error",
                        Message = "User or Email not found"
                    });
                }
            }

            if (!await _userManager.CheckPasswordAsync(checkUser, request.Password))
                return Unauthorized(new Response()
                {
                    Title = "Error",
                    Message = "Password is wrong"
                });

            var result = await _signInManager.PasswordSignInAsync(checkUser, request.Password, true, true);
            if (!result.Succeeded)
                return BadRequest(new Response()
                {
                    Title = "Error",
                    Message = "Sign in failed"
                });
            //Lấy ra tên các role của user đăng nhập
            var roleNames = await _userManager.GetRolesAsync(checkUser);
            //Lấy ra list các role
            var listRoles = await _roleManager.Roles
                .Where(r => r.Name != null && roleNames.Contains(r.Name))
                .ToListAsync();

            if (checkUser.UserName == null) return Unauthorized();
            
            var authClaims = new List<Claim>()
            {
                new(ClaimTypes.Name, checkUser.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Add các roleNames của user đăng nhập vào authClaims:
            authClaims.AddRange(
                roleNames.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

            //Tao token khi đăng nhập
            var tokenOptions = GetToken(authClaims);

            return Ok(new AuthResponse()
            {
                IsAuthSuccessful = true,
                Token = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
                Expiration = tokenOptions.ValidTo
            });

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
}