using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Post;

namespace Contracts.Models;

public interface IPostRepository
{
    Task<List<PostDto>> GetAllPosts();
    Task<PostDto> CreatePost(CreatePostRequest request);
    Task<PostDto> UpdatePost(string id, UpdatePostRequest request);
    Task DeletePost(string id);
    Task<PostDto> GetPostsById(string id);
}