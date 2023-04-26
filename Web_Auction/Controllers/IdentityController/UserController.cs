using Contracts.Identity;
using Entities;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.IdentityController;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository repository, ILogger<UserController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _repository.GetAllUsers();
            _logger.LogDebug("Returned all users from database");
            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllRoles action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _repository.GetUserById(id);
            _logger.LogDebug("Returned user has {Id} from database", id);
            return Ok(user);
        }
        catch (Exception e)
        {
           _logger.LogError("Something went wrong inside GetUserById action : {Message}", e.Message);
           return StatusCode(500, new Response()
           {
               Title = "Internal Server Error",
               Message = e.Message
           });
        }
    }

    

    [HttpPost("register-for-admin")]
    public async Task<IActionResult> CreateUserForAdmin([FromBody] CreateUserRequest request)
    {
        try
        {
            if(!ModelState.IsValid) throw new Exception("User information is not valid");
            var user = await _repository.CreateNewUserForAdmin(request);
            _logger.LogDebug("User created successfully");
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside CreateUser action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
    

    
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            if(!ModelState.IsValid) throw new Exception("User information is not valid");
            var user =await _repository.UpdateUser(id, request);
            _logger.LogDebug("User id : {Id} updated successfully", id);
            return Ok(user);

        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside UpdateUser action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            await _repository.DeleteUser(id);
            _logger.LogDebug("User id {Id} deleted successfully", id);
            return Ok(new Response()
            {
                Title = "Success",
                Message = "User deleted successfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeleteUser action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }


}