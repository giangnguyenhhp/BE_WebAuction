using Entities.Identity.DataTransferObject;

namespace Entities.Models.DataTransferObject;

public class PostDto
{
    public Guid PostId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Content { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public UserDto? User { get; set; }
}