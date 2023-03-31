using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.CardMember;

namespace Contracts.Models;

public interface ICardMemberRepository
{
    Task<List<CardMemberDto>> GetAllCardMembers();
    Task<CardMemberDto> CreateCardMember(CreateCardMemberRequest request);
    Task UpdateCardMember(string id,UpdateCardMemberRequest request);
    Task DeleteCardMember(string id);
}