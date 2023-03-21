using AutoMapper;
using Contracts.Models;
using Entities;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Category;
using Microsoft.EntityFrameworkCore;

namespace Repository.Models;

public class CategoryRepository : ICategoryRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;

    public CategoryRepository(MasterDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategories()
    {
        var categories = await _dbContext.Categories.ToListAsync();

        var result = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return result;
    }
    
    public async Task<CategoryDto?> GetCategoryById(Guid id)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c=>c.CategoryId == id);
        if (category == null)
        {
            throw new Exception("Category not found");
        }
        
        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }

    public async Task<CategoryDto> CreateCategory(CreateCategoryRequest request)
    {
        if (_dbContext.Categories.Select(c=>c.CategoryName).Contains(request.CategoryName))
        {
            throw new Exception("Category already exists");
        }
        var category = new Category()
        {
            CategoryId = Guid.NewGuid(),
            CategoryName = request.CategoryName,
            Description = request.Description
        };
        
        
        await _dbContext.Categories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }

    public async Task UpdateCategory(Guid id, UpdateCategoryRequest request)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c=>c.CategoryId == id);
        if (category == null)
        {
            throw new Exception("Category not found");
        }
        
        if (_dbContext.Categories.Select(c=>c.CategoryName).Contains(request.CategoryName))
        {
            throw new Exception("Category already exists");
        }
        
        category.CategoryName = request.CategoryName;
        category.Description = request.Description;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCategory(Guid id)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c=>c.CategoryId == id);
        if (category == null)
        {
            throw new Exception("Category not found");
        }

        _dbContext.Remove(category);
        await _dbContext.SaveChangesAsync();
    }


}