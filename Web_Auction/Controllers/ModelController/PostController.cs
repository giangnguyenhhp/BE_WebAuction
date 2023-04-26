using Contracts.Models;
using Entities;
using Entities.Models.RequestModels.Post;
using Microsoft.AspNetCore.Mvc;

namespace Web_Auction.Controllers.ModelController;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostRepository _repository;
    private readonly ILogger<PostController> _logger;

    public PostController(IPostRepository repository, ILogger<PostController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        try
        {
            var listPosts = await _repository.GetAllPosts();
            _logger.LogDebug("Returned all posts from database");
            return Ok(listPosts);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetAllPosts action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(string id)
    {
        try
        {
            var post = await _repository.GetPostsById(id);
            _logger.LogDebug("Returned post has id : {Id} from database",id);
            return Ok(post);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside GetPostById action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        try
        {
            var post = await _repository.CreatePost(request);
            _logger.LogDebug("Created post");
            return Ok(post);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside CreatePost action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdatePost(string id,[FromBody] UpdatePostRequest request)
    {
        try
        {
            var post = await _repository.UpdatePost(id, request);
            _logger.LogDebug("Post has id : {Id} updated",id);
            return Ok(post);
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside UpdatePost action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeletePost(string id)
    {
        try
        {
            await _repository.DeletePost(id);
            _logger.LogDebug("Post has id : {Id} deleted",id);
            return Ok(new Response()
            {
                Title = "Success",
                Message = "Post has been deleted"
            });
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong inside DeletePost action : {Message}", e.Message);
            return StatusCode(500, new Response()
            {
                Title = "Internal Server Error",
                Message = e.Message
            });
        }
    }
}