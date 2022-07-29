using RestSharp;

namespace WebApiJwt.Data
{
    public class ApiClient
    {
        private static readonly string? aspNetCoreVariable = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        public static RestClient? Rest { get; }
        public static HttpClient? Http { get; }
        static ApiClient()
        {
            string? baseAddress = aspNetCoreVariable?.Split(";")[0];
            Rest = baseAddress == null ? null : new(baseAddress);
            Http = baseAddress == null ? null : new() { BaseAddress = new Uri(baseAddress) };
        }
    }
}
