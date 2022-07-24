using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using UseCases.API.Dto;

namespace WebRzrPgAppUser.Pages.PhoneBook
{
    public class IndexModel : PageModel
    {
        static readonly string apiAddress = "https://localhost:7068/";//»ÎË http://localhost:5068/
        private static readonly string path = "api/Phones";
        public IList<PhoneDto>? Phone { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = null!;
        public async Task OnGetAsync()
        {
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                Phone = JsonConvert.DeserializeObject<List<PhoneDto>>(result);
            }
        }
    }
}
