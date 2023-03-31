using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Identity.Models;

namespace Entities.Models;

public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ProductId { get; set; }

    [Required]
    public string? ProductName { get; set; }

    public double? PriceOpen { get; set; }

    public string? Description { get; set; }

    public bool? IsApproved { get; set; }

    public LotProduct? LotProduct { get; set; }

    public Category? Category { get; set; }

    public AppUser? User { get; set; }

    public List<ProductPhoto>? ProductPhotos { get; set; }
}