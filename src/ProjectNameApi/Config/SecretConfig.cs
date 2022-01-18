namespace ProjectNameApi.Config
{
    /// <summary>
    ///     Secret store mapped from the ApiConfig object from appsettings.json or its relevant counterpart for given
    ///     environment
    /// </summary>
    public class SecretConfig
    {
        /// <summary>
        ///     Connection string for DB connection
        /// </summary>
        public string ConnectionString { get; set; }
        
        /// <summary>
        ///     Secret used for generating JWT tokens
        /// </summary>
        public string JwtAuthSecret { get; set; }

        /// <summary>
        ///     JWT token expiry time in minutes
        /// </summary>
        public int JwtExpiryMinutes { get; set; }
        
        /// <summary>
        ///     Password salt used scrambling passwords before storing them
        /// </summary>
        public string PasswordSalt { get; set; }

        public string ExampleSecret { get; set; }
    }
}