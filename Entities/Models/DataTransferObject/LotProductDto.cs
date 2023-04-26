namespace Entities.Models.DataTransferObject;

public class LotProductDto
{
    public Guid LotProductId { get; set; }

    public double? PriceLotOpen { get; set; }

    public DateTime? TimeStarted { get; set; }

    public DateTime? TimeEnded { get; set; }

    public DateTime? TimeRemaining { get; set; }

    public double? PriceOfferMax { get; set; }

    public List<ProductDto>? Products { get; set; }
}