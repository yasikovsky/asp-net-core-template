using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectNameApi.Entities.Users;
using ProjectNameApi.Enums;
using ProjectNameApi.Helpers;

namespace ProjectNameApi.Controllers.Attributes
{
    public class RequireRoleAttribute : TypeFilterAttribute
    {
        public RequireRoleAttribute(RoleType role) : base(typeof(RequireRoleFilter))
        {
            Arguments = new object[] {role};
        }
    }

    public class RequireRoleFilter : IAuthorizationFilter
    {
        private readonly RoleType _requiredRole;

        public RequireRoleFilter(RoleType requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userClaim = context.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "user");

            if (userClaim == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var user = JsonSerializer.Deserialize<User>(userClaim.Value, ApiHelper.JsonSerializerOptions);

            if (!User.AuthorizeRole(user.Role, _requiredRole)) context.Result = new ForbidResult();
        }
    }
}