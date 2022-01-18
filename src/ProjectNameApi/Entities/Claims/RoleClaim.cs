using System;
using System.Text.Json.Serialization;
using Dapper;
using ProjectNameApi.Enums;

namespace ProjectNameApi.Entities.Claims
{
    public class RoleClaim
    {
        [Key] public Guid RoleClaimId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoleType Role { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ClaimType ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}