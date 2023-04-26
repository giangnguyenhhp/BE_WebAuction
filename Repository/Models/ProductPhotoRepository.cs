using System.Net.Http.Headers;
using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class ProductPhotoRepository : ControllerBase, IProductPhotoRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProductPhotoRepository(MasterDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<ProductPhotoDto>> GetProductPhotos(string id)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductId.ToString() == id);
        if (product == null)
        {
            throw new Exception("Product not found.");
        }

        var productPhotos = await _dbContext.ProductPhotos
            .Where(x => x.Product.ProductId.ToString() == id).ToListAsync();

        var result = _mapper.Map<List<ProductPhotoDto>>(productPhotos);
        return result;
    }

    public async Task DeletePhotoAsync(string id)
    {
        var productPhoto =
            await _dbContext.ProductPhotos.FirstOrDefaultAsync(x => x.ProductPhotoId.ToString() == id);
        if (productPhoto == null)
        {
            throw new Exception("Photo not found");
        }

        _dbContext.ProductPhotos.Remove(productPhoto);
        await _dbContext.SaveChangesAsync();
    }
}