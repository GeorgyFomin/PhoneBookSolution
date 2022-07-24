using Microsoft.AspNetCore.Mvc;
using UseCases.API.Authentication;


namespace WebPhoneBook.Controllers
{

    public class AuthenticateController : Controller
    {
        static readonly string apiAddress = "https://localhost:7252/";//Или http://localhost:5252/
        private static readonly string path = "api/Authenticate";

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
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path + "/register-user", model);
            //response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode ? RedirectToAction("LoginUser", "Authenticate") :
                Content($"Error! User creation failed! Status Code:{response.StatusCode}");
            //Content(response.StatusCode.ToString()); //RedirectToAction("LoginAdmin", "Authenticate");
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
            //response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode ? RedirectToAction("LoginAdmin", "Authenticate") :
                Content($"Error! Admin creation failed! {response.StatusCode}");
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
            //response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode ? RedirectToAction("Index", "Phones") :
                Content($"Error! User login failed! Status Code:{response.StatusCode}");
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
            //response.EnsureSuccessStatusCode();
            return response.IsSuccessStatusCode ? RedirectToAction("Index", "Phones") : Content($"Error! Admin login failed! Status Code:{response.StatusCode}");

        }
    }
}
