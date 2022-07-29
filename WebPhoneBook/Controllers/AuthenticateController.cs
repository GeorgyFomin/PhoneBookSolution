using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using UseCases.API.Authentication;


namespace WebPhoneBook.Controllers
{

    public class AuthenticateController : Controller
    {
        static readonly string apiAddress = "https://localhost:7252/";//Или http://localhost:5252/
        private static readonly string path = "api/Auth";
        public static string? JwtToken { get; set; }
        public static string? Role { get; set; }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            RestClient restClient = new(apiAddress);
            RestRequest? restRequest = new(path + "/register-user", Method.Post);
            if (JwtToken != null)
                restRequest.AddHeader("Authorization", $"Bearer {JwtToken}");
            restRequest.AddJsonBody(model);
            RestResponse? restResponse = await restClient.ExecutePostAsync(restRequest);
            if (restResponse.IsSuccessful)
                return RedirectToAction("LoginUser", "Authenticate");
            else
            {
                string? content = restResponse.Content;
                if (content != null)
                {
                    RegisterResponse? registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(content);
                    if (registerResponse != null && registerResponse.Message != null)
                    {
                        return Content(registerResponse.Message);
                    }
                }
                return Content(restResponse.StatusCode.ToString());
            }
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path + "/register-admin", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("LoginAdmin", "Authenticate");
            }
            else
            {
                RegisterResponse? registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(await response.Content.ReadAsStringAsync());
                return
                    Content(registerResponse != null && registerResponse.Message != null ? registerResponse.Message :
                    $"Error! Admin creation failed! {await response.Content.ReadAsStringAsync()}");
            }
        }

        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path + "/Login", model);
            if (response.IsSuccessStatusCode)
            {
                JwtToken = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync())?.Token;
                Role = UserRoles.User;
                return RedirectToAction("Index", "Phones");
            }
            return Content($"Error! User login failed! Status Code:{response.StatusCode}");
        }
        [HttpGet]
        public IActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAdmin(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path + "/Login", model);
            if (response.IsSuccessStatusCode)
            {
                JwtToken = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync())?.Token;
                Role = UserRoles.Admin;
                return RedirectToAction("Index", "Phones");
            }
            return Content($"Error! Admin login failed! Status Code:{response.StatusCode}");

        }
    }
}
