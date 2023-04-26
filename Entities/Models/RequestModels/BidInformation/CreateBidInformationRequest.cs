namespace Entities.Models.RequestModels.BidInformation;

public class CreateBidInformationRequest
{
    public double? PriceLotOffer { get; set; }
    
    public Guid? LotProductId { get; set; }

}