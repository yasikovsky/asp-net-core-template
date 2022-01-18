using System;
using Dapper;

namespace ProjectNameApi.Entities.Users
{
    public class RefreshToken
    {
        [Key] public Guid RefreshTokenId { get; set; }

        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}