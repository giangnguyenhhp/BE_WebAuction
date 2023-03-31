using System.Security.Claims;
using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Identity.Models;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repository.Models;

public class ProductRepository :ControllerBase,IProductRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(MasterDbContext dbContext, IMapper mapper, UserManager<AppUser> userManager, IHttpContextAccessor http, ILogger<ProductRepository> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userManager = userManager;
        _http = http;
        _logger = logger;
    }

    public async Task<ProductDto> CreateNewProductAsync(CreateProductRequest request)
    {

        var userName = _userManager.GetUserName(_http?.HttpContext.User);
        if (userName == null) throw new Exception("User not found.");
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }

        var product = new Product
        {
            IsApproved = false,
            Category = category,
            User = user
        };
        _mapper.Map(request, product);


        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<ProductDto>(product);
        return result;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _dbContext.Products.OrderBy(p => p.IsApproved)
            .Include(p => p.Category)
            .Include(p=>p.User)
            .ToListAsync();
        var result = _mapper.Map<IEnumerable<ProductDto>>(products);
        return result;
    }

    public async Task<ProductDto> GetProductByIdAsync(Guid id)
    {
        var product = await _dbContext.Products.Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null)
        {
            throw new Exception("Product not found.");
        }

        var result = _mapper.Map<ProductDto>(product);
        return result;
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _dbContext.Products.Include(x=>x.User).FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null)
        {
            throw new Exception("Product not found.");
        }

        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }

        _mapper.Map(request, product);
        product.Category = category;

        await _dbContext.SaveChangesAsync();
        var result = _mapper.Map<ProductDto>(product);
        return result;
    }

    public async Task<Product> DeleteProductAsync(Guid id)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (product == null)
        {
            throw new Exception("Product not found.");
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        return product;
    }
}