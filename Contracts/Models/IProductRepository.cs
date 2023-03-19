using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace Contracts.Models;

public interface IProductRepository
{
    Task<ProductDto> CreateNewProductAsync(CreateProductRequest request);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto> GetProductByIdAsync(Guid id);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductRequest request);
    Task<Product> DeleteProductAsync(Guid id);
}