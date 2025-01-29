using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SaalGallery.Utilities.Extensions;

public static class SwaggerExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opts =>
        {
            opts.SwaggerDoc("v1", new OpenApiInfo { Title = "Saal API", Version = "v1" });
            opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            opts.OperationFilter<AuthOperationFilter>();
        });
    }

    public class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var isAuthorized = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(row => row is AuthorizeAttribute);
            var allowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata.Any(row => row is AllowAnonymousAttribute);

            if (isAuthorized && !allowAnonymous)
            {
                operation.Responses.Add("401", new OpenApiResponse() { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse() { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement()
                {
                    {
                       new OpenApiSecurityScheme
                       {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                       }, new string[]{}
                    }
                });
            }
        }
    }
}