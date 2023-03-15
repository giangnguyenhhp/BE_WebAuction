using AutoMapper;
using Entities.Identity.DataTransferObject;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;
using Microsoft.AspNetCore.Identity;

namespace Web_Auction.AutoMapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<IdentityRole,RoleDto>();
        CreateMap<RoleRequest, IdentityRole>();
        CreateMap<AppUser, UserDto>();
        CreateMap<CreateUserRequest, AppUser>();
        CreateMap<UpdateUserRequest, AppUser>();
    }
}