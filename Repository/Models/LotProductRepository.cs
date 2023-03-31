using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.LotProduct;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class LotProductRepository : ILotProductRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;

    public LotProductRepository(MasterDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LotProductDto>> GetAllLotProducts()
    {
        var lotProducts = await _dbContext.LotProducts
            .Include(x => x.Products).ToListAsync();
         
        foreach (var lotProduct in lotProducts)
        {
           var priceMax = _dbContext.BidInformation.Where(x => x.LotProduct.LotProductId == lotProduct.LotProductId)
                .Max(x => x.PriceLotOffer);
           lotProduct.PriceOfferMax = priceMax;
        }

        var result = _mapper.Map<IEnumerable<LotProductDto>>(lotProducts);
        return result;
    }

    public async Task<LotProductDto> CreateLotProductAsync(CreateLotProductRequest request)
    {

        var products = await _dbContext.Products.Where(x =>
            request.ProductIds != null && request.ProductIds.Contains(x.ProductId.ToString())).ToListAsync();
        var priceOpen = products.Sum(product => product.PriceOpen);
        var lotProduct = new LotProduct()
        {
            Products = products,
            PriceLotOpen = priceOpen,
            TimeStarted = request.TimeStarted,
            TimeEnded = request.TimeEnded
        };

        await _dbContext.LotProducts.AddAsync(lotProduct);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<LotProductDto>(lotProduct);
        return result;
    }

    public async Task UpdateLotProductAsync(Guid id, UpdateLotProductRequest request)
    {
        var lotProduct = await _dbContext.LotProducts
            .Include(x=>x.Products).FirstOrDefaultAsync(x => x.LotProductId == id);
        if (lotProduct == null)
        {
            throw new Exception("LotProduct not found");
        }

        var products = await _dbContext.Products.Where(x =>
            request.ProductIds != null && request.ProductIds.Contains(x.ProductId.ToString())).ToListAsync();
        var priceOpen = products.Sum(product => product.PriceOpen);

        lotProduct.Products = products;
        lotProduct.PriceLotOpen = priceOpen;
        lotProduct.TimeStarted = request.TimeStarted;
        lotProduct.TimeEnded = request.TimeEnded;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteLotProductAsync(Guid id)
    {
        var lotProduct = await _dbContext.LotProducts
            .Include(x=>x.Products)
            .FirstOrDefaultAsync(x => x.LotProductId == id);
        if (lotProduct == null)
        {
            throw new Exception("LotProduct not found");
        }

        _dbContext.LotProducts.Remove(lotProduct);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<LotProductDto> GetLotProductById(Guid id)
    {
        var lotProduct = await _dbContext.LotProducts.FirstOrDefaultAsync(x => x.LotProductId == id);
        if (lotProduct == null)
        {
            throw new Exception("LotProduct not found");
        }
        
        var result = _mapper.Map<LotProductDto>(lotProduct);
        return result;
    }
}