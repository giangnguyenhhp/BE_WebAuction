using Contracts.Models;
using Entities;
using Entities.Models.RequestModels.BidInformation;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;

[ApiController]
[Route("/api/[controller]")]
public class BidInformationController : ControllerBase
{
    private readonly IBidInformationRepository _repository;
    private readonly ILogger<BidInformationController> _logger;

    public BidInformationController(IBidInformationRepository repository, ILogger<BidInformationController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBidInformation()
    {
        try
        {
            var listBidInformation = await _repository.GetAllBidInformation();
            _logger.LogDebug("Return all BidInformation from database");
            return Ok(listBidInformation);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllBidInformation action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateBidInformation([FromBody] CreateBidInformationRequest request)
    {
        try
        {
            var bidInformation = await _repository.CreateBidInformation(request);
            _logger.LogDebug("Create bidInformation successfully");
            return Ok(bidInformation);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside CreateBidInformation action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteBidInformation(string id)
    {
        try
        {
            await _repository.DeleteBidInformation(id);
            _logger.LogDebug("Delete bidInformation successfully");
            return Ok(new Response()
            {
                Title = "Successfully",
                Message = "Bid deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeleteBidInformation action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}