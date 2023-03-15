using Contracts.Identity;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.IdentityController;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RoleController> _logger;

    public RoleController(IRoleRepository roleRepository, ILogger<RoleController> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var roles = await _roleRepository.GetAllRoles();
            _logger.LogDebug("Returned all roles from database");
            return Ok(roles);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllRoles action : {Message}", e.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewRole([FromBody] RoleRequest? request)
    {
        try
        {
            if (request is null)
            {
                _logger.LogError("Role object sent from client is null");
                return BadRequest("Role object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid role object sent from client");
                return BadRequest("Invalid model object");
            }

            var role = await _roleRepository.CreateNewRole(request);
            _logger.LogDebug("Role {Name} created successfully", request.Name);
            return Ok(role);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside CreateNewRole action : {Message}", e.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateRole(string id, RoleRequest? request)
    {
        try
        {
            if (request is null)
            {
                _logger.LogError("Role object sent from client is null");
                return BadRequest("Role object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid role object sent from client");
                return BadRequest("Invalid model object");
            }

            var role = await _roleRepository.UpdateRole(id, request);
            _logger.LogDebug("Role {Name} updated successfully", request.Name);
            return Ok(role);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside UpdateRole action : {Message}", e.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        try
        {
            var role = await _roleRepository.DeleteRole(id);
            _logger.LogDebug("Role {Name} deleted", role.Name);
            return Ok(role);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeleteRole action : {Message}", e.Message);
            return StatusCode(500, "Internal Server Error");
        }
    }
}