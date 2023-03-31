using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Comment;

namespace Contracts.Models;

public interface ICommentRepository
{
    Task<CommentDto> PostComment(CreateCommentRequest request);
    Task<List<CommentDto>> GetCommentsByUserId(string id);
    Task<List<CommentDto>> GetAllComments();
    Task DeleteComment(string id);
}