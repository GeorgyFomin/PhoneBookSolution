namespace UseCases.API.Core
{
    public class ApiClient
    {
        public static readonly string address = "https://localhost:7252/";//Или http://localhost:5252///
        // Берет переменную окружения активного процесса, в котором класс создается!!
        //Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";")[0];
        public static readonly string authPath = "api/Auth";
        public static readonly string phonesPath = "api/Phones";
        public static string? UsedRole { get; set; }
        public static string? JwtToken { get; set; }

    }
}
