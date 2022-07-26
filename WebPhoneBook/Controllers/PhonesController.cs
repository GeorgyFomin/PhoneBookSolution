using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using UseCases.API.Dto;

namespace WebPhoneBook.Controllers
{
    public class PhonesController : Controller
    {
        static readonly string apiAddress = "https://localhost:7252/";//Или http://localhost:5252/
        private static readonly string path = "api/Phones";
        readonly RestClient restClient = new(apiAddress);
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
        public async Task<IActionResult> AdminIndex()
        {
            return await Index();
        }
        public async Task<IActionResult> UserIndex()
        {
            return await Index();
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
        // GET: Phones/Details/id
        public async Task<IActionResult> Details(int? id) => await GetPhoneById(id);
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
            RestRequest? restRequest = new(path, Method.Post);
            if (AuthenticateController.JwtToken != null)
                restRequest.AddHeader("Authorization", $"Bearer {AuthenticateController.JwtToken}");
            restRequest.AddJsonBody(phoneDto);
            RestResponse? restResponse = await restClient.ExecutePostAsync(restRequest);
            return restResponse.IsSuccessful ? RedirectToAction(nameof(UserIndex)) : Content(restResponse.StatusCode.ToString());
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
                RestRequest? restRequest = new(path + $"/{phoneDto.Id}", Method.Put);
                if (AuthenticateController.JwtToken != null)
                    restRequest.AddHeader("Authorization", $"Bearer {AuthenticateController.JwtToken}");
                restRequest.AddJsonBody(phoneDto);
                RestResponse? restResponse = await restClient.ExecutePutAsync(restRequest);
                if (!restResponse.IsSuccessful)
                {
                    return Content(restResponse.StatusCode.ToString());
                }
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
            return RedirectToAction(nameof(AdminIndex));
        }

        // GET: Phones/Delete/id
        public async Task<IActionResult> Delete(int? id) => await GetPhoneById(id);

        // POST: Phones/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            RestRequest? restRequest = new(path + $"/{id}", Method.Delete);
            if (AuthenticateController.JwtToken != null)
                restRequest.AddHeader("Authorization", $"Bearer {AuthenticateController.JwtToken}");
            RestResponse? restResponse = await restClient.ExecuteAsync(restRequest);
            return restResponse.IsSuccessful ? RedirectToAction(nameof(AdminIndex)) : Content(restResponse.StatusCode.ToString());
        }
    }
}
