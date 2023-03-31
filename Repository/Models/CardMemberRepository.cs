using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Identity.Models;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.CardMember;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class CardMemberRepository : ICardMemberRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<AppUser> _userManager;

    public CardMemberRepository(MasterDbContext dbContext, IMapper mapper, IHttpContextAccessor http, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _http = http;
        _userManager = userManager;
    }

    public async Task<List<CardMemberDto>> GetAllCardMembers()
    {
        var listCardMembers = await _dbContext.CardMembers
            .Include(x=>x.User)
            .ToListAsync();

        var result = _mapper.Map<List<CardMemberDto>>(listCardMembers);
        return result;
    }

    public async Task<CardMemberDto> CreateCardMember(CreateCardMemberRequest request)
    {
        var userName = _userManager.GetUserName(_http?.HttpContext.User);
        if (userName == null) throw new Exception("User not found.");
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        if (_dbContext.CardMembers.Select(x=>x.User.Id).Contains(user.Id))
        {
            throw new Exception("User already have a card member");
        }
        
        var cardMember = new CardMember
        {
            User = user
        };
        _mapper.Map(request, cardMember);

        await _dbContext.CardMembers.AddAsync(cardMember);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<CardMemberDto>(cardMember);
        return result;
    }

    public async Task UpdateCardMember(string id, UpdateCardMemberRequest request)
    {
        var cardMember = await _dbContext.CardMembers.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.CardId.ToString() == id);
        if (cardMember==null)
        {
            throw new Exception("CardMember not found.");
        }

        _mapper.Map(request, cardMember);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCardMember(string id)
    {
        var cardMember = await _dbContext.CardMembers.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.CardId.ToString() == id);
        if (cardMember==null)
        {
            throw new Exception("CardMember not found.");
        }
        
        _dbContext.CardMembers.Remove(cardMember);
        await _dbContext.SaveChangesAsync();
    }
}