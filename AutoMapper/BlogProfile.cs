using AutoMapper;
using BlogApp.Dtos.Response;
using BlogApp.Entities;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.AutoMapper;

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
        CreateMap<AddPostDto, Post>();
        CreateMap<UpdatePostDto, Post>();
    }
}