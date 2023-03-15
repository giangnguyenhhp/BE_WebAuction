using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class AuctionInformation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid AuctionId { get; set; }

    public string? AuctionName { get; set; }

    public string? Description { get; set; }

    public DateTime TimeStarted { get; set; }

    public DateTime TimeEnded { get; set; }

    public DateTime TimeRemaining { get; set; }

    public List<LotProduct>? LotProducts { get; set; }
}