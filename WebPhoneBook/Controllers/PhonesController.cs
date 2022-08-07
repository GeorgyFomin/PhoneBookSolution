using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Core;
using UseCases.API.Dto;

namespace WebPhoneBook.Controllers
{
    public class PhonesController : Controller
    {
        // GET: Phones
        public async Task<IActionResult> Index()
        {
            List<PhoneDto>? phoneDtos = new();
            HttpResponseMessage response = await ApiClient.Http.GetAsync(ApiClient.phonesPath);
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

            HttpResponseMessage response = await ApiClient.Http.GetAsync(ApiClient.phonesPath + $"/{id}");
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
                ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            }
            HttpResponseMessage? response = await ApiClient.Http.PostAsJsonAsync(ApiClient.phonesPath, phoneDto);
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
                    ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
                }
                HttpResponseMessage? response = await ApiClient.Http.PutAsJsonAsync(ApiClient.phonesPath + $"/{phoneDto.Id}", phoneDto);
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
                ApiClient.Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ApiClient.JwtToken);
            }
            HttpResponseMessage? response = await ApiClient.Http.DeleteAsync(ApiClient.phonesPath + $"/{id}");
            return response.IsSuccessStatusCode ? RedirectToAction(nameof(Index)) : Content(response.StatusCode.ToString());
        }
    }
}
