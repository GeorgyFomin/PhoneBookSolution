using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebRzrPgAppUser.Pages.PhoneBook
{
    public class DetailsModel : PageModel
    {
        static readonly string apiAddress = "https://localhost:7068/";//»ÎË http://localhost:5068/
        private static readonly string path = "api/Phones";
        public PhoneDto? Phone { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path + $"/{id}");
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                Phone = JsonConvert.DeserializeObject<PhoneDto>(result);
            }
            if (Phone == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
