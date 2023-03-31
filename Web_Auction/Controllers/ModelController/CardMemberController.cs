using Contracts.Models;
using Entities;
using Entities.Models.RequestModels.CardMember;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;

[ApiController]
[Route("/api/[controller]")]
public class CardMemberController : ControllerBase
{
    private readonly ICardMemberRepository _repository;
    private readonly ILogger<CardMemberController> _logger;

    public CardMemberController(ICardMemberRepository repository, ILogger<CardMemberController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllCardMembers()
    {
        try
        {
            var listCardMembers = await _repository.GetAllCardMembers();
            _logger.LogDebug("Returned all CardMembers from database");
            return Ok(listCardMembers);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllCardMember action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult> CreateCardMember([FromBody] CreateCardMemberRequest request)
    {
        try
        {
            var cardMember = await _repository.CreateCardMember(request);
            _logger.LogDebug("Created CardMember");
            return Ok(cardMember);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside CreateCardMember action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message,
            });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult> UpdateCardMember(string id, [FromBody] UpdateCardMemberRequest request)
    {
        try
        {
            await _repository.UpdateCardMember(id,request);
            _logger.LogDebug("CardMember has id : {Id} updated",id);
            return Ok(new Response()
            {
                Title = "Success",
                Message = "CardMember has been updated"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside UpdateCardMember action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteCardMember(string id)
    {
        try
        {
            await _repository.DeleteCardMember(id);
            _logger.LogDebug("CardMember has id : {Id} deleted",id);
            return Ok(new Response()
            {
                Title = "Success",
                Message = "CardMember has been deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeleteCardMember action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}