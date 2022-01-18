using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectNameApi.Entities.Claims;
using ProjectNameApi.Enums;
using ProjectNameApi.Helpers;

namespace ProjectNameApi.Controllers.Attributes
{
    public class RequireClaimAttribute : TypeFilterAttribute
    {
        /// <summary>
        ///     Requires current user to have a claim of supplied type (with or without a value)
        /// </summary>
        /// <param name="claimType">Claim type</param>
        public RequireClaimAttribute(ClaimType claimType) : base(typeof(RequireClaimFilter))
        {
            Arguments = new object[] {new[] {claimType}, new string[0]};
        }

        /// <summary>
        ///     Requires current user to user to have a claim of supplied type with at least one of supplied claim values
        /// </summary>
        /// <param name="claimType">Claim type</param>
        /// <param name="claimValues">Claim values</param>
        public RequireClaimAttribute(ClaimType claimType, params string[] claimValues) : base(
            typeof(RequireClaimFilter))
        {
            Arguments = new object[] {new[] {claimType}, claimValues};
        }

        /// <summary>
        ///     Requires current user to have at least one claim of supplied types (with or without a value)
        /// </summary>
        /// <param name="claimTypes">Claim types</param>
        public RequireClaimAttribute(params ClaimType[] claimTypes) : base(typeof(RequireClaimFilter))
        {
            Arguments = new object[] {claimTypes, new string[0]};
        }
    }

    public class RequireClaimFilter : IAuthorizationFilter
    {
        private readonly string[] _claimValues;
        private readonly ClaimType[] _requiredClaims;

        public RequireClaimFilter(ClaimType[] claims, string[] claimValues)
        {
            _requiredClaims = claims;
            _claimValues = claimValues ?? new string[0];
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasRequiredClaim = false;

            var appClaims = context.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "appClaims");

            if (appClaims == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var claims =
                JsonSerializer.Deserialize<List<AppClaim>>(appClaims.Value,
                    ApiHelper.JsonSerializerOptions);

            foreach (var claim in claims)
            {
                // If it's an administrator then approve and skip checking anything else 
                if (claim.ClaimType == ClaimType.IsAdministrator)
                    return;

                // If this claim isn't in the required list continue with the next one
                if (_requiredClaims.All(a => a != claim.ClaimType))
                    continue;

                // If this claim is required, but no value is required 
                if (_claimValues.Length == 0)
                {
                    hasRequiredClaim = true;
                    break;
                }

                // If this claim is required, but it doesn't have any of the required values
                if (_claimValues.All(a => a == claim.Value))
                    continue;

                hasRequiredClaim = true;
                break;
            }

            if (!hasRequiredClaim) context.Result = new ForbidResult();
        }
    }
}