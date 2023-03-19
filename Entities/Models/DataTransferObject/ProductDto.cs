namespace Entities.Models.DataTransferObject;

public class ProductDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }

    public double PriceOpen { get; set; }

    public string? Description { get; set; }

    public bool IsApproved { get; set; }
}