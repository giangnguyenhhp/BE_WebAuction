using Entities.Identity.DataTransferObject;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Identity;

namespace Contracts.Identity;

public interface IRoleRepository
{
    public Task<IEnumerable<RoleDto>> GetAllRoles();
    public Task<RoleDto> CreateNewRole(RoleRequest request);
    public Task<RoleDto> UpdateRole(string id, RoleRequest request);
    public Task<IdentityRole> DeleteRole(string id);
}