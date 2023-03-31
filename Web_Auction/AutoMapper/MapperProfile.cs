using AutoMapper;
using Entities.Identity.DataTransferObject;
using Entities.Identity.Models;
using Entities.Identity.RequestModels;
using Entities.Models;
using Entities.Models.DataTransferObject;
using Entities.Models.RequestModels.CardMember;
using Entities.Models.RequestModels.Post;
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
        
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();

        CreateMap<Category, CategoryDto>();

        CreateMap<LotProduct, LotProductDto>();

        CreateMap<Comment, CommentDto>();

        CreateMap<CardMember, CardMemberDto>();
        CreateMap<CreateCardMemberRequest, CardMember>();
        CreateMap<UpdateCardMemberRequest, CardMember>();

        CreateMap<Contact, ContactDto>();

        CreateMap<Post, PostDto>();
        CreateMap<CreatePostRequest, Post>();
        CreateMap<UpdatePostRequest, Post>();
    }
}