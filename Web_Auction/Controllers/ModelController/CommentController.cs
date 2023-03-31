using Contracts.Models;
using Entities;
using Entities.Models.RequestModels.Comment;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;
[ApiController]
[Route("/api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository _repository;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentRepository repository, ILogger<CommentController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllComments()
    {
        try
        {
            var listComments = await _repository.GetAllComments();
            _logger.LogDebug("Returned all comments from database");
            return Ok(listComments);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllComments action : {Message}",e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpGet("/by-user-id/{id}")]
    public async Task<IActionResult> GetCommentByUserId(string id)
    {
        try
        {
            var listComment = await _repository.GetCommentsByUserId(id);
            _logger.LogDebug("Get all comment by user id : {Id} from database",id);
            return Ok(listComment);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in GetCommentByUserId action : {Message}",e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult> PostComment([FromBody] CreateCommentRequest request)
    {
        try
        {
            var comment = await _repository.PostComment(request);
            _logger.LogDebug("Comment Created");
            return Ok(comment);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in PostComment action : {Message}",e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(string id)
    {
        try
        {
            await _repository.DeleteComment(id);
            _logger.LogDebug("Comment Deleted");
            return Ok(new Response()
            {
                Title = "Successfully",
                Message = "Comment Deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong in DeleteComment action : {Message}",e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}