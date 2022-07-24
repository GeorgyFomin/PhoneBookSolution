using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UseCases.API.Dto;
namespace WebAppRoles.Controllers
{
    public class PhonesController : Controller
    {
        static readonly string apiAddress = "https://localhost:7068/";//Или http://localhost:5068/
        private static readonly string path = "api/Phones";
        [AllowAnonymous]
        // GET: Phones
        public async Task<IActionResult> Index()
        {
            List<PhoneDto>? phoneDtos = new();
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                phoneDtos = JsonConvert.DeserializeObject<List<PhoneDto>>(result);
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
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                phoneDto = JsonConvert.DeserializeObject<PhoneDto>(result);
            }
            return phoneDto == null ? NotFound() : View(phoneDto);
        }
        [AllowAnonymous]
        // GET: Phones/Details/id
        public async Task<IActionResult> Details(int? id) => await GetPhoneById(id);
        [Authorize(Roles = "admin, user")]
        // GET: Phones/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Phones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,PhoneNumber,Id")] PhoneDto phoneDto)
        {
            if (!ModelState.IsValid)
            {
                return View(phoneDto);
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path, phoneDto);
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "admin")]
        // GET: Phones/Edit/id
        public async Task<IActionResult> Edit(int? id) => await GetPhoneById(id);
        // POST: Phones/Edit/id
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
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
                HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
                HttpResponseMessage response = await client.PutAsJsonAsync(path + $"/{phoneDto.Id}", phoneDto);
                response.EnsureSuccessStatusCode();
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
        [Authorize(Roles = "admin")]
        // GET: Phones/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetPhoneById(id);
        // POST: Phones/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.DeleteAsync(path + $"/{id}");
            response.EnsureSuccessStatusCode();
            return RedirectToAction(nameof(Index));
        }
    }
}
