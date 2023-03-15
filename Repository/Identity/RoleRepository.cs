using AutoMapper;
using Contracts.Identity;
using Entities;
using Entities.Identity.DataTransferObject;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Repository.Identity;

public class RoleRepository : IRoleRepository
{
    private readonly MasterDbContext _dbContext;
    private readonly ILogger<RoleRepository> _logger;
    private readonly IMapper _mapper;

    public RoleRepository(MasterDbContext dbContext, ILogger<RoleRepository> logger, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRoles()
    {
        var roles = await _dbContext.Roles.ToListAsync();
        var rolesResult = _mapper.Map<IEnumerable<RoleDto>>(roles);
        return rolesResult;
    }

    public async Task<RoleDto> CreateNewRole(RoleRequest? request)
    {
        if (request is null)
        {
            _logger.LogError("Role object sent from client is null");
            throw new Exception("Role object is null");
        }
        
        // var role = new IdentityRole()
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Name = request.Name,
        //     NormalizedName = request.Name?.ToUpper(),
        //     ConcurrencyStamp = Guid.NewGuid().ToString()
        // };

        var role = _mapper.Map<IdentityRole>(request);
        role.Id = Guid.NewGuid().ToString();
        role.NormalizedName = role.Name?.ToUpper();
        role.ConcurrencyStamp = Guid.NewGuid().ToString();

        if (_dbContext.Roles.Select(x => x.NormalizedName).Contains(request.Name?.ToUpper()))
        {
            throw new Exception($"Role name {request.Name} already created");
        }

        await _dbContext.Roles.AddAsync(role);
        await _dbContext.SaveChangesAsync();

        var roleResult = _mapper.Map<RoleDto>(role);
        return await Task.FromResult(roleResult);
    }

    public async Task<RoleDto> UpdateRole(string id, RoleRequest request)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
        {
            throw new Exception($"Role has id : {id} not found");
        }

        role = _mapper.Map(request,role);
        role.NormalizedName = request.Name?.ToUpper();

        await _dbContext.SaveChangesAsync();

        var roleResult = _mapper.Map<RoleDto>(role);
        return await Task.FromResult(roleResult);
    }

    public async Task<IdentityRole> DeleteRole(string id)
    {
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
        if (role == null)
        {
            throw new Exception($"Role has id : {id} not found");
        }

        _dbContext.Roles.Remove(role);
        await _dbContext.SaveChangesAsync();
        return await Task.FromResult(role);
    }
}