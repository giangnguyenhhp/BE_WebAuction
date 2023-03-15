using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Identity.Models;

namespace Entities.Models;

public class LotProduct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid LotProductId { get; set; }

    public double PriceLotOpen { get; set; }

    public double PriceLotOffer { get; set; }

    public List<Product>? Products { get; set; }

    public List<AppUser>? Users { get; set; }

    public AuctionInformation? AuctionInformation { get; set; }
}