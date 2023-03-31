using Entities.Identity.DataTransferObject;

namespace Entities.Models.DataTransferObject;

public class CommentDto
{
    public Guid CommentId { get; set; }

    public string? Content { get; set; }

    public UserDto? User { get; set; }
}