using Microsoft.AspNetCore.Identity;
using SpiderNet.Application.Interfaces;
using SpiderNet.Application.Interfaces.Repositories;
using SpiderNet.Application.Interfaces.Services;
using SpiderNet.Application.Services;
using SpiderNet.Domain.Entities;
using SpiderNet.Infrastructure.Repositories;
using SpiderNet.Infrastructure.Services;
using SpiderNet.Infrastructure.Settings;

namespace spidernet_be.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IPasswordValidationService, PasswordValidationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IReactionRepository, ReactionRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ICommentReactionRepository, CommentReactionRepository>();
        services.AddScoped<ICommentService, CommentService>();
        
        return services;
    }
}