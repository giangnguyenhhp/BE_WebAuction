using Contracts.Models;
using Entities;
using Entities.Models.RequestModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryRepository repository, ILogger<CategoryController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        try
        {
            var categories = await _repository.GetAllCategories();
            _logger.LogDebug("Return all categories from database");
            return Ok(categories);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in GetAllCategories action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        try
        {
            var category = await _repository.GetCategoryById(id);
            _logger.LogDebug("Return Category has id : {Id} from database", id);
            return Ok(category);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in GetAllCategories action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        try
        {
            var category = await _repository.CreateCategory(request);
            _logger.LogDebug("Category created successfully");
            return Ok(category);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in CreateCategory action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        try
        {
            await _repository.UpdateCategory(id, request);
            _logger.LogDebug("Category has id : {Id} updated successfully", id);
            return Ok(new Response()
            {
                Title = "Successfully",
                Message = "Category has been updated successfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in UpdateCategory action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        try
        {
            await _repository.DeleteCategory(id);
            _logger.LogDebug("Category has id : {Id} deleted successfully", id);
            return Ok(new Response()
            {
                Title = "Successfully",
                Message = "Category has been deleted successfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in DeleteCategory action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}