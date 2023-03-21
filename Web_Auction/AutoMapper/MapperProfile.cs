using AutoMapper;
using Entities.Identity.DataTransferObject;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.Product;
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
        
        CreateMap<CreateProductRequest, Product>();
        CreateMap<Product, ProductDto>();
        CreateMap<UpdateProductRequest, Product>();

        CreateMap<Category, CategoryDto>();
    }
}