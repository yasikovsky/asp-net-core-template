using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectNameApi.Config
{
    public class SwaggerAuthFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerAttributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true);
            var methodAttributes = context.MethodInfo?.GetCustomAttributes(true);

            var controllerAuthorized =
                controllerAttributes != null && controllerAttributes.OfType<AuthorizeAttribute>().Any();
            var methodAuthorized = methodAttributes != null && methodAttributes.OfType<AuthorizeAttribute>().Any();
            var methodAllowAnonymous =
                methodAttributes != null && methodAttributes.OfType<AllowAnonymousAttribute>().Any();

            if (!controllerAuthorized && !methodAuthorized)
                return;

            if (controllerAuthorized && methodAllowAnonymous)
                return;

            operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new()
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
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                }
            };
        }
    }
}