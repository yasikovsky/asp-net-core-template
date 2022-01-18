using Microsoft.AspNetCore.Hosting;

namespace ProjectNameApi.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IWebHostEnvironmentExt
    {
        public static bool IsBeta(this IWebHostEnvironment env)
        {
            return env.EnvironmentName.ToLower() == "beta";
        }
    }
}