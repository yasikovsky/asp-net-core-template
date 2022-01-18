using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using ProjectNameApi.Controllers.Attributes;
using ProjectNameApi.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectNameApi.Config
{
    public class SwaggerRoleFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerAttributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true);
            var methodAttributes = context.MethodInfo?.GetCustomAttributes(true);

            var controllerRole = RoleType.None;
            var methodRole = RoleType.None;

            if (controllerAttributes != null && controllerAttributes.OfType<RequireRoleAttribute>().Any())
                controllerRole = (RoleType) controllerAttributes.OfType<RequireRoleAttribute>().First().Arguments[0];

            if (methodAttributes != null && methodAttributes.OfType<RequireRoleAttribute>().Any())
                methodRole = (RoleType) methodAttributes.OfType<RequireRoleAttribute>().First().Arguments[0];

            var reqRole = RoleType.None;

            if (controllerRole != RoleType.None)
                reqRole = controllerRole;

            if (methodRole != RoleType.None)
                reqRole = methodRole;

            var allowedRoles = Enum.GetValues(typeof(RoleType)).Cast<RoleType>().Where(a => a >= reqRole).ToArray();

            if (reqRole != RoleType.None) operation.Description += $"Allowed roles: {string.Join(", ", allowedRoles)}";
        }
    }
}