using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace Entities.Identity.Models;

public class AppUser : IdentityUser
{
    public string? Address { get; set; }

    public List<Product>? ProductsSold { get; set; }

    public CardMember? CardMember { get; set; }

    public List<Comment>? Comments { get; set; }

    // public List<LotProduct>? LotProducts { get; set; }

    public List<BidInformation>? BidInformationS { get; set; }

    [NotMapped] public IEnumerable<IdentityRole>? Roles { get; set; }
}