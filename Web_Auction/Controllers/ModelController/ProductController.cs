using Contracts.Models;
using Entities;
using Entities.Models.RequestModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductRepository repository, ILogger<ProductController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _repository.GetAllProductsAsync();
            _logger.LogDebug("Return all products from database");
            return Ok(products);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllProducts action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        try
        {
            var product = await _repository.GetProductByIdAsync(id);
            _logger.LogDebug("Return product has id : {Id} from database", id);
            return Ok(product);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetProductById action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateNewProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid product object sent from client");
                return BadRequest("Invalid model object");
            }

            var product = await _repository.CreateNewProductAsync(request);
            _logger.LogDebug("Product has been created");
            return Ok(product);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside CreateNewProduct action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var product = await _repository.UpdateProductAsync(id, request);
            _logger.LogDebug("Product has id : {Id} updated",id);
            return Ok(product);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside UpdateProduct action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
             await _repository.DeleteProductAsync(id);
            _logger.LogDebug("Product has id : {Id} deleted",id);
            return Ok(new Response()
            {
                Title = "Success",
                Message = "Product has been deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeleteProduct action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}