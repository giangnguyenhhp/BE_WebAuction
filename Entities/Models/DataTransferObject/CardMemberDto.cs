using Entities.Identity.DataTransferObject;

namespace Entities.Models.DataTransferObject;

public class CardMemberDto
{
    public Guid CardId { get; set; }

    public string? NameMember { get; set; }

    public string? AddressMember { get; set; }

    public string? PhoneNumberMember { get; set; }

    public double? Deposit { get; set; }

    public UserDto? User { get; set; }
}