using Entities.Identity.DataTransferObject;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;

namespace Contracts.Identity;

public interface IUserRepository
{
    public Task<IEnumerable<UserDto>> GetAllUsers();
    public Task<UserDto?> CreateNewUserForAdmin(CreateUserRequest request);
    public Task<UserDto> UpdateUser(string id, UpdateUserRequest request);
    public Task<AppUser> DeleteUser(string id);
    public Task<UserDto> GetUserById(string id);
}