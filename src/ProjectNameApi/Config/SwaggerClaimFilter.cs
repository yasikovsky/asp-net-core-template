using System.Collections.Generic;
using System.Linq;
using ProjectNameApi.Extensions;
using Microsoft.OpenApi.Models;
using ProjectNameApi.Controllers.Attributes;
using ProjectNameApi.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectNameApi.Config
{
    public class SwaggerClaimFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerAttributes = context.MethodInfo?.DeclaringType?.GetCustomAttributes(true);
            var methodAttributes = context.MethodInfo?.GetCustomAttributes(true);

            var attributeList = new List<RequireClaimAttribute>();

            if (controllerAttributes != null && controllerAttributes.OfType<RequireClaimAttribute>().Any())
                attributeList.AddRange(controllerAttributes.OfType<RequireClaimAttribute>());

            if (methodAttributes != null && methodAttributes.OfType<RequireClaimAttribute>().Any())
                attributeList.AddRange(methodAttributes.OfType<RequireClaimAttribute>());

            if (!attributeList.Any())
                return;

            var reqClaims = new List<ClaimType>();
            var reqClaimsWithValue = new Dictionary<ClaimType, List<string>>();

            foreach (var attribute in attributeList)
            {
                // If second argument is null then it's a list of claims without values
                if (attribute.Arguments[1] == null)
                {
                    reqClaims.AddRange((ClaimType[]) attribute.Arguments[0]);
                    continue;
                }

                var claimType = ((ClaimType[]) attribute.Arguments[0])[0];
                var claimValues = (string[]) attribute.Arguments[1];

                reqClaimsWithValue.Add(claimType, claimValues.ToList());
            }

            if (reqClaims.Any())
            {
                if (string.IsNullOrEmpty(operation.Description)) operation.Description += "<br/>";
                operation.Description +=
                    $"Required claims: <ul>{reqClaims.JoinWithPrefixOrSuffix("<li>", "</li>")}</ul>";
            }

            if (reqClaimsWithValue.Any())
            {
                var requiredClaimsAsText = "";

                if (string.IsNullOrEmpty(operation.Description)) operation.Description += "<br/>";
                operation.Description += "<p>Required claims with values: <ul>";

                foreach (var claim in reqClaimsWithValue)
                {
                    requiredClaimsAsText += $"<li>{claim.Key}<ul>";
                    requiredClaimsAsText += $"{claim.Value.JoinWithPrefixOrSuffix("<li>", "</li>")}</ul></li>";
                }

                operation.Description += $"{requiredClaimsAsText}</ul></p>";
            }
        }
    }
}