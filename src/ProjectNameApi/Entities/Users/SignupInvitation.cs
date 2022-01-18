using System;
using Dapper;

namespace ProjectNameApi.Entities.Users
{
    public class SignupInvitation
    {
        [Key] public Guid SignupInvitationId { get; set; }

        public Guid Token { get; set; }
        public string InvitedEmail { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}