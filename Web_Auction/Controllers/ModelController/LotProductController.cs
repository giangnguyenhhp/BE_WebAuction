using Contracts.Models;
using Entities;
using Entities.Models.RequestModels.LotProduct;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;

[ApiController]
[Route("/api/[controller]")]
public class LotProductController : ControllerBase
{
    private readonly ILotProductRepository _repository;
    private readonly ILogger<LotProductController> _logger;

    public LotProductController(ILotProductRepository repository, ILogger<LotProductController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLotProductsAsync()
    {
        try
        {
            var lotProducts = await _repository.GetAllLotProducts();
            _logger.LogDebug("Returned all LotProducts from database");
            return Ok(lotProducts);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in GetAllLotProducts action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLotProductById(Guid id)
    {
        try
        {
            var lotProduct = await _repository.GetLotProductById(id);
            _logger.LogDebug("Returned LotProduct has id : {Id} from database",id);
            return Ok(lotProduct);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in GetLotProductById action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateLotProduct([FromBody] CreateLotProductRequest request)
    {
        try
        {
            var lotProduct = await _repository.CreateLotProductAsync(request);
            _logger.LogDebug("Created LotProduct Successfully");
            return Ok(lotProduct);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in CreateLotProduct action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateLotProduct(Guid id, [FromBody] UpdateLotProductRequest request)
    {
        try
        {
            await _repository.UpdateLotProductAsync(id, request);
            _logger.LogDebug("Update LotProduct has id : {Id} Successfully",id);
            return Ok(new Response()
            {
                Title = "Success",
                Message = "UpdateSlotProduct Successfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in UpdateLotProduct action : {Message}",e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteLotProduct(Guid id)
    {
        try
        {
            await _repository.DeleteLotProductAsync(id);
            _logger.LogDebug("DeletelotProduct has id : {Id} Successfully", id);
            return Ok(new Response()
            {
                Title = "Success",
                Message = "Delete LotProduct Successfully"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in DeleteLotProduct action : {Message}",e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}