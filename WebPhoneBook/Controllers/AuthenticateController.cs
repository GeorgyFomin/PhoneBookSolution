﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using UseCases.API.Authentication;


namespace WebPhoneBook.Controllers
{

    public class AuthenticateController : Controller
    {
        static readonly string apiAddress = "https://localhost:7252/";//Или http://localhost:5252/
        private static readonly string path = "api/Authenticate";
        public static string? JwtToken { get; set; }

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
            return restResponse.IsSuccessful ? RedirectToAction("LoginUser", "Authenticate") : Content(restResponse.StatusCode.ToString());
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
            if (response.IsSuccessStatusCode)
            {
                JwtToken = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync())?.Token;
                return RedirectToAction("UserIndex", "Phones");
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
                return RedirectToAction("AdminIndex", "Phones");
            }
            return Content($"Error! Admin login failed! Status Code:{response.StatusCode}");

        }
    }
}
