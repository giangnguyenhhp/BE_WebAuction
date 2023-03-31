namespace Entities.Models.DataTransferObject;

public class ContactDto
{
    public Guid ContactId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Message { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? DateSent { get; set; }
}