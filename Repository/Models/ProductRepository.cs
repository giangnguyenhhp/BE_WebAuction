﻿using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class ProductRepository : IProductRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProductRepository(MasterDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ProductDto> CreateNewProductAsync(CreateProductRequest request)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId);
        if (category == null)
        {
            throw new Exception("Category not found.");
        }

        var product = new Product();
        _mapper.Map(request, product);
        product.ProductId = Guid.NewGuid();
        product.Category = category;

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<ProductDto>(product);
        return result;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _dbContext.Products.OrderBy(p => p.IsApproved)
            .Include(p => p.Category)
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
        var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
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