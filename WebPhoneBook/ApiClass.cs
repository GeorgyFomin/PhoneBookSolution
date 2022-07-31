using RestSharp;

namespace WebPhoneBook
{
    public class ApiClient
    {
        private const string baseAddress = "https://localhost:7252/";//Или http://localhost:5252///
        public static RestClient Rest { get; }
        public static HttpClient Http { get; }
        static ApiClient()
        {
            Rest = new(baseAddress);
            Http = new() { BaseAddress = new Uri(baseAddress) };
        }
    }
}
