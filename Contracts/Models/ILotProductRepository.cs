using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.LotProduct;

namespace Contracts.Models;

public interface ILotProductRepository
{
    Task<IEnumerable<LotProductDto>> GetAllLotProducts();
    Task<LotProductDto> CreateLotProductAsync(CreateLotProductRequest request);
    Task UpdateLotProductAsync(Guid id, UpdateLotProductRequest request);
    Task DeleteLotProductAsync(Guid id);
    Task<LotProductDto> GetLotProductById(Guid id);
}