using AutoMapper;
using BlogApp.Core.Common.Helpers;
using BlogApp.Core.Dtos.Request;
using BlogApp.Core.Dtos.Response;
using BlogApp.Core.Utils;
using BlogApp.Domain.Entities;

namespace BlogApp.Core.Common.Mapping;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        //Category Mappers
        CreateMap<Category, CategoryResponseDto>()
            .ForMember(dest => dest.Posts, 
                opt => opt.MapFrom(src => src.Posts));
        CreateMap<AddCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();
        
        //Comments Mapper
        CreateMap<Comment, CommentResponseDto>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User.Username));
        CreateMap<CommentDto, Comment>();
        CreateMap<UpdateCommentDto, Comment>();
        
        //Likes Mapper
        CreateMap<Like, LikeResponseDto>()
            .ForMember(dest=>dest.Username,
                opt=>opt.MapFrom(src=>src.User.Username));
        CreateMap<LikeDto, Like>();
        
        //Tags Mapper
        CreateMap<Tag, TagResponseDto>();
        CreateMap<AddTagDto, Tag>();
        CreateMap<UpdateTagDto, Tag>();
        
        //User Mapper
        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.PasswordHash,
                opt => opt.MapFrom(src => PasswordHelper.HashPassword(src.Password)))
            .ForMember(dest => dest.EmailVerificationToken,
                opt => opt.MapFrom(src => EmailVerificationUtils.GenerateVerificationToken()))
            .ForMember(dest => dest.EmailVerificationTokenExpiresAt,
                opt => opt.MapFrom(src=>EmailVerificationUtils.GetTokenExpiration(24)))
            .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.IsEmailVerified,
                opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(_ => DateTime.Now));
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.AccessToken,
                opt => opt.MapFrom(_ => "Pending verification - Check your email for verification link"));
        CreateMap<LoginRequestDto, User>();
        CreateMap<User, ProfileResponseDto>();
        CreateMap<UpdateProfileRequestDto, User>();
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>));


        //Post Mapper
        CreateMap<Post, PostResponseDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.LikesCount,
                opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CommentsCount,
                opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src.Tags.Select(t => t.Name).ToList()));
        CreateMap<AddPostDto, Post>()
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(_ => DateTime.Now));
        CreateMap<UpdatePostDto, Post>()
            .ForMember(dest=>dest.Slug,
                opt=> opt.MapFrom(src=>SlugUtils.GenerateSlug(src.Title)))
            .ForMember(dest=>dest.UpdatedAt,
                opt=>opt.MapFrom(_ => DateTime.Now));
    }
}