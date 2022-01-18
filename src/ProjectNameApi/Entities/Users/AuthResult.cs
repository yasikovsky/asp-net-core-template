namespace ProjectNameApi.Entities.Users
{
    public class AuthResult
    {
        public AuthResult(AuthStatus status)
        {
            Status = status;
        }

        // Constructor for successful authentication
        public AuthResult(User user, string accessToken, string refreshToken)
        {
            Status = AuthStatus.Success;
            User = user;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public User User { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public AuthStatus Status { get; set; }
    }

    public enum AuthStatus
    {
        InvalidUsernameOrPassword = 0,
        Success = 1,
        UnknownError = 2,
        EmailNotConfirmed = 3,
        NeedsTwoFactor = 4,
        InvalidRefreshToken = 5,
        UserDoesntExist = 6
    }
}