namespace UseCases.API.Authentication
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; } = DateTime.MinValue;
    }
}
