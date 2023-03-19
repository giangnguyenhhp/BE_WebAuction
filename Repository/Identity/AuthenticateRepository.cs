using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Contracts.Identity;
using EmailService.SendMailServices;
using Entities;
using Entities.Identity.AuthenticateRequestModels;
using Entities.Identity.DataTransferObject;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Repository.Identity;

public class AuthenticateRepository : ControllerBase, IAuthenticateRepository
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<AuthenticateRepository> _logger;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;
    private readonly MasterDbContext _dbContext;

    public AuthenticateRepository(IConfiguration configuration, UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager,
        ILogger<AuthenticateRepository> logger, IMapper mapper, IEmailSender emailSender, MasterDbContext dbContext)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _logger = logger;
        _mapper = mapper;
        _emailSender = emailSender;
        _dbContext = dbContext;
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

    public async Task<IActionResult> LoginAsync(LoginRequest request)
    {
        if(request.UserNameOrEmail==null || request.Password==null) throw new Exception("User name or email and password is required.");
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

        if (!await _userManager.IsEmailConfirmedAsync(checkUser))
        {
            return Unauthorized(new Response()
            {
                Title = "Error",
                Message = "Email is not confirmed. Please check email to confirm your account."
            });
        }

        var result = await _signInManager.PasswordSignInAsync(checkUser, request.Password, true, true);
        if (!result.Succeeded)
            return Unauthorized(new Response()
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
            Token = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
            Expiration = tokenOptions.ValidTo
        });
    }
    public async Task<IActionResult> EmailConfirmation(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest("Invalid Email Confirmation Request");
        }

        var confirmResult = await _userManager.ConfirmEmailAsync(user, token);
        if (!confirmResult.Succeeded) return BadRequest("Invalid Email Confirmation Request");
        return Ok();
    }

    public async Task<IActionResult> RegisterForClient(CreateUserRequest request)
    {
        var user = new AppUser();
        _mapper.Map(request, user);
        user.Id = Guid.NewGuid().ToString();

        if (_userManager.Users.Select(x => x.UserName).Contains(request.UserName))
        {
            throw new Exception($"User name {request.UserName} already taken");
        }

        if (request.Password != null)
        {
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) throw new Exception("Failed to create user");
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var param = new Dictionary<string, string>()
        {
            { "token", token },
            { "email", request.Email }
        };
        var callback = QueryHelpers.AddQueryString("http://localhost:4200/authentication/emailConfirmation", param);
        var message = new Message(new[] { user.Email }, "Email Confirmation Token", callback);
        await _emailSender.SendEmailAsync(message);

        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName: "User"));
        }

        if (await _roleManager.RoleExistsAsync("User"))
        {
            await _userManager.AddToRoleAsync(user, "User");
        }

        await _dbContext.SaveChangesAsync();

        var userResult = _mapper.Map<UserDto>(user);
        return Ok(userResult);
    }

    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        if (request.Email == null) throw new Exception("Email is required");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var param = new Dictionary<string, string>()
        {
            { "token", token },
            { "email", request.Email }
        };
        var callBack2 = QueryHelpers.AddQueryString("http://localhost:4200/authentication/resetPassword", param);

        var message = new Message(new[] { user.Email }, "Reset Password Token", callBack2);
        await _emailSender.SendEmailAsync(message);
        return Ok(new Response()
        {
            Message = "Reset Password Token sent",
            Title = "Successfully"
        });
    }

    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (request.Email == null) throw new Exception("Email is required");
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        if (request.Token == null) throw new Exception("Token is required");

        if (request is not { Token: { }, Password: { } }) return BadRequest("Token and Password are required");
        var resetPassResult = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (resetPassResult.Succeeded) return Ok(new Response()
        {
            Message = "Your password has been changed.",
            Title = "Successfully"
        });
        var errors = resetPassResult.Errors.Select(e => e.Description);
        foreach (var error in errors)
        {
            throw new Exception(error);
        }
        return BadRequest(new Response()
        {
            Title = "Error",
            Message = "Your password has not been changed.",
        });
    }


}