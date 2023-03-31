using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Contact
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ContactId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Message { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DateSent { get; set; }
}