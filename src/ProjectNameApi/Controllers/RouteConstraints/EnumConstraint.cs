using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ProjectNameAPi.Controllers.RouteConstraints
{
    public class EnumConstraint<T>: IRouteConstraint where T: struct, Enum
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // retrieve the candidate value
            var candidate = values[routeKey]?.ToString();

            if (candidate == null)
                return false;
            
            // attempt to parse the candidate to the required Enum type, and return the result
            return Enum.TryParse(candidate, out T _);
        }
    }
}