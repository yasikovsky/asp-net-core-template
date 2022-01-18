using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Http;
using ProjectNameApi.Enums;
using ProjectNameApi.Helpers;

namespace ProjectNameApi.Entities.Users
{
    [Table("user")]
    public class User
    {
        [Key] public Guid UserId { get; set; }

        public string Username { get; set; }

        [JsonIgnore] public string PasswordHash { get; set; }

        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }

        public RoleType Role { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string SystemLanguageCode { get; set; }

        public static bool AuthorizeRole(RoleType currentRole, RoleType requiredRole)
        {
            return requiredRole <= currentRole;
        }

        public static User GetCurrentUser(HttpContext context)
        {
            var userClaim = context.User.Claims.FirstOrDefault(a => a.Type == "user");

            if (userClaim == null) return null;

            var user = JsonSerializer.Deserialize<User>(userClaim.Value, ApiHelper.JsonSerializerOptions);

            return user;
        }
    }
}