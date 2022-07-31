using RestSharp;

namespace WebApiJwt.Data
{
    public class Client
    {
        static readonly string? Address = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";")[0];
        public RestClient? Rest { get; } = Address == null ? null : new RestClient(Address);
        public HttpClient? Http { get; } = Address == null ? null : new HttpClient() { BaseAddress = new Uri(Address) };
    };
}
