using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UseCases.API.Authentication;
using UseCases.API.Core;

namespace WebPhoneBook.Controllers
{

    public class AuthenticateController : Controller
    {
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

            if (ApiClient.JwtToken != null)
            {
                ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            }
            HttpResponseMessage? response = await ApiClient.Http.PostAsJsonAsync(ApiClient.authPath + "/register-user", model);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("LoginUser", "Authenticate");
            }
            else
            {
                HttpContent? content = response.Content;
                if (content != null)
                {
                    RegisterResponse? registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(await content.ReadAsStringAsync());
                    if (registerResponse != null && registerResponse.Message != null)
                    {
                        return Content(registerResponse.Message);
                    }
                }
                return Content(response.StatusCode.ToString());
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

            HttpResponseMessage response = await ApiClient.Http.PostAsJsonAsync(ApiClient.authPath + "/register-admin", model);
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

            HttpResponseMessage response = await ApiClient.Http.PostAsJsonAsync(ApiClient.authPath + "/Login", model);
            if (response.IsSuccessStatusCode)
            {
                ApiClient.JwtToken = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync())?.Token;
                ApiClient.UsedRole = UserRoles.User;
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

            HttpResponseMessage response = await ApiClient.Http.PostAsJsonAsync(ApiClient.authPath + "/Login", model);
            if (response.IsSuccessStatusCode)
            {
                ApiClient.JwtToken = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync())?.Token;
                ApiClient.UsedRole = UserRoles.Admin;
                return RedirectToAction("Index", "Phones");
            }
            return Content($"Error! Admin login failed! Status Code:{response.StatusCode}");

        }
    }
}
