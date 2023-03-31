using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Identity.Models;

namespace Entities.Models;

public class BidInformation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid BidId { get; set; }
    
    public double? PriceLotOffer { get; set; }
    
    public LotProduct? LotProduct { get; set; }

    public AppUser? AppUser { get; set; }
    
}