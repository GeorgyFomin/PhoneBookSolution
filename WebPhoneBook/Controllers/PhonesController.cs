using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Core;
using UseCases.API.Dto;

namespace WebPhoneBook.Controllers
{
    public class PhonesController : Controller
    {
        private readonly HttpClient httpClient = ApiClient.Http;
        //private readonly IHttpClientFactory _httpClientFactory;
        //public PhonesController(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClientFactory = httpClientFactory;
        //    httpClient = _httpClientFactory.CreateClient("ApiClient");
        //}
        // GET: Phones
        public async Task<IActionResult> Index()
        {
            List<PhoneDto>? phoneDtos = new();
            HttpResponseMessage response =
            await ApiClient.Http.GetAsync(ApiClient.phonesPath);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                phoneDtos = JsonConvert.DeserializeObject<List<PhoneDto>>(result);
                ViewBag.Role = ApiClient.UsedRole;
            }
            return View(phoneDtos);
        }
        async Task<IActionResult> GetPhoneById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PhoneDto? phoneDto = null;

            HttpResponseMessage response =
            await httpClient.GetAsync(ApiClient.phonesPath + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                phoneDto = JsonConvert.DeserializeObject<PhoneDto>(result);
            }
            return phoneDto == null ? NotFound() : View(phoneDto);
        }
        // GET: Phones/Details/id
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.Role = ApiClient.UsedRole;
            return await GetPhoneById(id);
        }

        // GET: Phones/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Phones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,PhoneNumber,Id")] PhoneDto phoneDto)
        {
            if (!ModelState.IsValid)
            {
                return View(phoneDto);
            }

            if (ApiClient.JwtToken != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            }
            HttpResponseMessage? response =
            await httpClient.PostAsJsonAsync(ApiClient.phonesPath, phoneDto);
            return response.IsSuccessStatusCode ? RedirectToAction(nameof(Index)) : Content(response.StatusCode.ToString());
        }
        // GET: Phones/Edit/id
        public async Task<IActionResult> Edit(int? id) => await GetPhoneById(id);

        // POST: Phones/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Name,PhoneNumber,Id")] PhoneDto phoneDto)
        {
            if (!ModelState.IsValid)
            {
                return View(phoneDto);
            }
            try
            {
                if (ApiClient.JwtToken != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
                }
                HttpResponseMessage? response =
                await httpClient.PutAsJsonAsync(ApiClient.phonesPath + $"/{phoneDto.Id}", phoneDto);
                if (!response.IsSuccessStatusCode)
                    return Content(response.StatusCode.ToString());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (GetPhoneById(phoneDto.Id).IsFaulted)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Phones/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetPhoneById(id);

        // POST: Phones/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (ApiClient.JwtToken != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            }
            HttpResponseMessage? response =
            await httpClient.DeleteAsync(ApiClient.phonesPath + $"/{id}");
            return response.IsSuccessStatusCode ? RedirectToAction(nameof(Index)) : Content(response.StatusCode.ToString());
        }
    }
}
