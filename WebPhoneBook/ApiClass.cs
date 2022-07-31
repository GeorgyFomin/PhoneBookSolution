using RestSharp;

namespace WebPhoneBook
{
    public class ApiClient
    {
        private const string baseAddress = "https://localhost:7252/";//Или http://localhost:5252///
        public static RestClient Rest { get; } = new(baseAddress);
        public static HttpClient Http { get; } = new() { BaseAddress = new Uri(baseAddress) };
    }
}
