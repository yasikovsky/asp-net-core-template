#nullable enable
using ProjectNameApi.Enums;

namespace ProjectNameApi.Entities.Claims
{
    public class AppClaim
    {
        public AppClaim(ClaimType claimType, string value)
        {
            ClaimType = claimType;
            Value = value;
        }

        public ClaimType? ClaimType { get; set; }
        public string? Value { get; set; }
    }
}