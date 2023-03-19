using AutoMapper;
using Contracts.Identity;
using Entities;
using Entities.Identity.DataTransferObject;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Repository.Identity;

public class UserRepository : IUserRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(MasterDbContext context, IMapper mapper, RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        _dbContext = context;
        _mapper = mapper;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        foreach (var user in users)
        {
            var listRoleNames = await _userManager.GetRolesAsync(user);
            var listRoles = await _roleManager.Roles
                .Where(r => r.Name != null && listRoleNames.Contains(r.Name))
                .ToListAsync();
            user.Roles = listRoles;
        }

        var usersResult = _mapper.Map<IEnumerable<UserDto>>(users);
        return usersResult;
    }

    public async Task<UserDto> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var listRoleNames = await _userManager.GetRolesAsync(user);
        var listRoles = await _roleManager.Roles
            .Where(r => r.Name != null && listRoleNames.Contains(r.Name))
            .ToListAsync();
        user.Roles = listRoles;
        
        var usersResult = _mapper.Map<UserDto>(user);
        return usersResult;
    }

    public async Task<UserDto?> CreateNewUserForAdmin(CreateUserRequest request)
    {
        var user = new AppUser();
        _mapper.Map(request, user);
        user.Id = Guid.NewGuid().ToString();


        if (_dbContext.Users.Select(x => x.UserName).Contains(request.UserName))
        {
            throw new Exception($"User name {request.UserName} already taken");
        }

        if (request.Password != null)
        {
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded) throw new Exception("Failed to create user");
        }

        //Kiểm tra xem role vừa nhập có trong database không
        var listRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        if (request.RoleNames != null && request.RoleNames.Any(roleName => !listRoles.Contains(roleName)))
        {
            throw new Exception("Role does not existed");
        }

        //Add các role cần thiết cho user vừa khởi tạo.
        if (request.RoleNames != null) await _userManager.AddToRolesAsync(user, request.RoleNames);

        await _dbContext.SaveChangesAsync();

        var userResult = _mapper.Map<UserDto>(user);
        return await Task.FromResult(userResult);
    }

    public async Task<UserDto> UpdateUser(string id, UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            throw new Exception($"User {id} not found in database");
        }

        if (request.UserName == user.UserName)
        {
            user.UserName = request.UserName;
        }

        if (request.Address == user.Address)
        {
            user.Address = request.Address;
        }

        if (request.UserName != user.UserName && _userManager.Users.Select(x => x.UserName).Contains(request?.UserName))
        {
            throw new Exception($"User name {request?.UserName} already taken");
        }

        _mapper.Map(request, user);
        user.NormalizedUserName = request?.UserName?.ToUpper();

        //Update Role cho user : Lấy ra các role cũ trong user và xóa toàn bộ
        //Kiểm tra xem role vừa nhập có trong database không,nếu có thì thực hiện add role cho user
        if (request?.RoleNames != null && request.RoleNames.Any())
        {
            var roleNames = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roleNames);

            var listRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            if (request.RoleNames.Any(roleName => !listRoles.Contains(roleName)))
            {
                throw new Exception("Role does not existed");
            }

            await _userManager.AddToRolesAsync(user, request.RoleNames);
        }

        await _userManager.UpdateAsync(user);
        await _dbContext.SaveChangesAsync();

        var userResult = _mapper.Map<UserDto>(user);
        return await Task.FromResult(userResult);
    }

    public async Task<AppUser> DeleteUser(string id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new Exception($"User {id} not found in database");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return await Task.FromResult(user);
    }
}