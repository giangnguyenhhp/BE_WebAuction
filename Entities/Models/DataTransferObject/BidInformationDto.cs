using Entities.Identity.DataTransferObject;

namespace Entities.Models.DataTransferObject;

public class BidInformationDto
{
    public Guid BidId { get; set; }
    
    public double? PriceLotOffer { get; set; }
    
    public LotProductDto? LotProduct { get; set; }

    public UserDto? AppUser { get; set; }
}