using BlogApp.Core.Interfaces.Repositories;
using BlogApp.Core.Interfaces.Services;
using BlogApp.Core.Services;
using BlogApp.Infrastructure.Data;
using BlogApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlogApp.Infrastructure.Configurations;

public static class ServiceCollectionConfig
{
    public static IServiceCollection AddAppRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICommentsService, CommentsService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<IEmailService, EmailService>();
        return services;
    }

    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}