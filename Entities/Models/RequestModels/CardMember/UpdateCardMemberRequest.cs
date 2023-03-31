namespace Entities.Models.RequestModels.CardMember;

public class UpdateCardMemberRequest
{
    public string? NameMember { get; set; }

    public string? AddressMember { get; set; }

    public string? PhoneNumberMember { get; set; }

    public double? Deposit { get; set; }
}