using System;
using System.Text.Json.Serialization;
using Dapper;
using ProjectNameApi.Enums;

namespace ProjectNameApi.Entities.Claims
{
    public class UserClaim
    {
        [Key] public Guid UserClaimId { get; set; }

        public Guid UserId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClaimType ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}