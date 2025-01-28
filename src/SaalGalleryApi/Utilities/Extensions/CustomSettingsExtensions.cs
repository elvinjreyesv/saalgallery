using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetryTool.OpenTelemetryConfig;
using SaalGallery.Mapper.Abstracts;
using SaalGallery.Models.Shared;
using SaalGallery.Repository;
using SaalGallery.Repository.Interfaces;
using SaalGallery.Services;
using SaalGallery.Services.Interfaces;
using SaalGalleryApi.Models.Shared;
using SaalGalleryApi.Services;
using SaalGalleryApi.Services.Interfaces;
using Scrutor;
using System.IdentityModel.Tokens.Jwt;

namespace SaalGallery.Utilities.Extensions;

public static class CustomSettingsExtensions
{
    public static IServiceCollection AddCustomAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OtelSettings>(configuration.GetSection("OtelSettings"));
        services.Configure<CustomAppSettings>(configuration.GetSection("CustomAppSettings"));
        services.Configure<ExternalConnectionSettings>(configuration.GetSection("ExternalConnectionStrings"));
        return services;
    }

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var appSettings = configuration.GetSection("CustomAppSettings:JwtConfig").Get<JwtConfig>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.SaveToken = true;
                opts.IncludeErrorDetails = true;
                opts.RequireHttpsMetadata = false;
                opts.RefreshOnIssuerKeyNotFound = true;
                opts.Audience = appSettings?.ValidAudience;
                opts.UseSecurityTokenValidators = true;

                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidIssuer = appSettings?.ValidIssuer,
                    ValidAudience = appSettings?.ValidAudience,
                    SignatureValidator = (token, parameters) =>
                    {
                        return new JwtSecurityToken(token);
                    }
                };
            });

        return services;
    }

    public static IServiceCollection AddScopedServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Repositories
        services.AddScoped<IGalleryRepository, GalleryRepository>();
        #endregion

        #region Services
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IAccountService, AccountService>();
        #endregion

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }

    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IMapper>()
            .AddClasses(x => x.AssignableTo(typeof(BaseCustomMapper<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelf()
            .WithTransientLifetime()

            .AddClasses(x => x.AssignableTo(typeof(IMapper<,>)))
            .AsMatchingInterface()
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelfWithInterfaces()
            .WithTransientLifetime()
        );

        return services;
    }
}