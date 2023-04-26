using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Identity.Models;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Post;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class PostRepository : IPostRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<AppUser> _userManager;

    public PostRepository(MasterDbContext dbContext, IMapper mapper, IHttpContextAccessor http,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _http = http;
        _userManager = userManager;
    }

    public async Task<List<PostDto>> GetAllPosts()
    {
        var listPosts = await _dbContext.Posts.Include(x => x.User).ToListAsync();

        var result = _mapper.Map<List<PostDto>>(listPosts);
        return result;
    }

    public async Task<PostDto> CreatePost(CreatePostRequest request)
    {
        var userName = _userManager.GetUserName(_http?.HttpContext.User);
        if (userName == null) throw new Exception("User not found.");
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var post = _mapper.Map<Post>(request);
        post.DateCreated = DateTime.Now;
        post.DateUpdated = post.DateCreated;
        post.User = user;

        await _dbContext.Posts.AddAsync(post);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<PostDto>(post);
        return result;
    }

    public async Task<PostDto> UpdatePost(string id, UpdatePostRequest request)
    {
        var post = await _dbContext.Posts.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.PostId.ToString() == id);
        if (post == null)
        {
            throw new Exception("Post not found.");
        }

        _mapper.Map(request, post);
        post.DateUpdated = DateTime.Now;
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<PostDto>(post);
        return result;
    }

    public async Task DeletePost(string id)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(x => x.PostId.ToString() == id);
        if (post == null)
        {
            throw new Exception("Post not found.");
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PostDto> GetPostsById(string id)
    {
        var post = await _dbContext.Posts.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.PostId.ToString() == id);
        if (post == null)
        {
            throw new Exception("Post not found.");
        }

        var result = _mapper.Map<PostDto>(post);
        return result;
    }
}