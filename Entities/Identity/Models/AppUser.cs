using System.ComponentModel.DataAnnotations.Schema;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace Entities.Identity.Models;

public class AppUser : IdentityUser
{
    public string? Address { get; set; }

    public List<Product>? Products { get; set; }

    public CardMember? CardMember { get; set; }

    public List<Comment>? Comments { get; set; }

    public List<LotProduct>? LotProducts { get; set; }

    [NotMapped] public IEnumerable<IdentityRole>? Roles { get; set; }
}