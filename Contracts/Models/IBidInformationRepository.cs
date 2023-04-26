using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.BidInformation;

namespace Contracts.Models;

public interface IBidInformationRepository
{
    Task<List<BidInformationDto>> GetAllBidInformation();
    Task<BidInformationDto> CreateBidInformation(CreateBidInformationRequest request);
    Task DeleteBidInformation(string id);
}