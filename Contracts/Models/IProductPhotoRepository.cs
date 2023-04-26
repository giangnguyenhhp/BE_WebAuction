using Entities.Models.DataTransferObject;

namespace Contracts.Models;

public interface IProductPhotoRepository
{
    Task<List<ProductPhotoDto>> GetProductPhotos(string id);
    Task DeletePhotoAsync(string id);
}