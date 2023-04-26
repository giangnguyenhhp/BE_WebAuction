namespace Entities.Models.DataTransferObject;

public class ProductPhotoDto
{
    public Guid ProductPhotoId { get; set; }

    public string? FileName { get; set; }

    public ProductDto? Product { get; set; }
}