using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Identity.Models;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Comment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class CommentRepository : ICommentRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<AppUser> _userManager;

    public CommentRepository(MasterDbContext dbContext, IMapper mapper, IHttpContextAccessor http,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _http = http;
        _userManager = userManager;
    }

    public async Task<CommentDto> PostComment(CreateCommentRequest request)
    {
        var userName = _userManager.GetUserName(_http?.HttpContext.User);
        if (userName == null) throw new Exception("User not found.");
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var comment = new Comment()
        {
            Content = request.Content,
            User = user,
        };

        await _dbContext.Comments.AddAsync(comment);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<CommentDto>(comment);
        return result;
    }

    public async Task<List<CommentDto>> GetCommentsByUserId(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var listComment = await _dbContext.Comments.Where(x => x.User != null && x.User.Id == id).ToListAsync();

        var result = _mapper.Map<List<CommentDto>>(listComment);
        return result;
    }

    public async Task<List<CommentDto>> GetAllComments()
    {
        var listComment = await _dbContext.Comments.Include(x => x.User).ToListAsync();

        var result = _mapper.Map<List<CommentDto>>(listComment);
        return result;
    }

    public async Task DeleteComment(string id)
    {
        var comment = await _dbContext.Comments.FirstOrDefaultAsync(x => x.CommentId.ToString() == id);
        if (comment == null)
        {
            throw new Exception("Comment not found.");
        }

        _dbContext.Remove(comment);
        await _dbContext.SaveChangesAsync();
    }
}