using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Identity.Models;

namespace Entities.Models;

public class CardMember
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid CardId { get; set; }

    public string? NameMember { get; set; }

    public string? AddressMember { get; set; }

    public string? PhoneNumberMember { get; set; }

    public double Deposit { get; set; }

    public string? UserId { get; set; }
    public AppUser? User { get; set; }
}