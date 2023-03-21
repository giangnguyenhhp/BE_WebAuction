using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Category;

namespace Contracts.Models;

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryDto>> GetAllCategories();
    Task<CategoryDto> CreateCategory(CreateCategoryRequest request);
    Task UpdateCategory(Guid id, UpdateCategoryRequest request);
    Task DeleteCategory(Guid id);
    Task<CategoryDto?> GetCategoryById(Guid id);
}