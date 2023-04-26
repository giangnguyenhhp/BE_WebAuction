using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Identity.Models;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.BidInformation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class BidInformationRepository : IBidInformationRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _http;
    private readonly UserManager<AppUser> _userManager;

    public BidInformationRepository(MasterDbContext dbContext, IMapper mapper, IHttpContextAccessor http,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _http = http;
        _userManager = userManager;
    }

    public async Task<List<BidInformationDto>> GetAllBidInformation()
    {
        var listBidInformation = await _dbContext.BidInformation
            .Include(x => x.LotProduct)
            .Include(x => x.AppUser)
            .ToListAsync();

        var result = _mapper.Map<List<BidInformationDto>>(listBidInformation);
        return result;
    }

    public async Task<BidInformationDto> CreateBidInformation(CreateBidInformationRequest request)
    {
        var userName = _userManager.GetUserName(_http?.HttpContext.User);
        if (userName == null) throw new Exception("User not found.");
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var lotProduct = await _dbContext.LotProducts.FirstOrDefaultAsync(x => x.LotProductId == request.LotProductId);
        if (lotProduct == null) throw new Exception("Lot product not found.");
        var priceLotOfferMax = await _dbContext.BidInformation
            .Where(x => x.LotProduct.LotProductId == lotProduct.LotProductId).Select(x => x.PriceLotOffer).MaxAsync();
        if (request.PriceLotOffer <= priceLotOfferMax)
        {
            throw new Exception($"Price Lot offer is less than Price lot offer max : {priceLotOfferMax}");
        }

        if (request.PriceLotOffer <= lotProduct.PriceLotOpen)
        {
            throw new Exception($"Price Lot offer must higher than Price Lot Open : {lotProduct.PriceLotOpen}");
        }

        var bidInformation = new BidInformation()
        {
            PriceLotOffer = request.PriceLotOffer,
            AppUser = user,
            LotProduct = lotProduct
        };

        await _dbContext.BidInformation.AddAsync(bidInformation);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<BidInformationDto>(bidInformation);
        return result;
    }

    public async Task DeleteBidInformation(string id)
    {
        var bid = await _dbContext.BidInformation
            .FirstOrDefaultAsync(x => x.BidId.ToString() == id);
        if (bid == null) throw new Exception("Bid not found.");

        _dbContext.BidInformation.Remove(bid);
        await _dbContext.SaveChangesAsync();
    }
}