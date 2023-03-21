namespace Entities.Models.DataTransferObject;

public class CategoryDto
{
    public Guid CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? Description { get; set; }
}