using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UseCases.API.Dto;

namespace WebRzrPgAppUser.Pages.PhoneBook
{
    public class CreateModel : PageModel
    {
        static readonly string apiAddress = "https://localhost:7068/";//»ÎË http://localhost:5068/
        private static readonly string path = "api/Phones";

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PhoneDto? Phone { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            HttpClient client = new() { BaseAddress = new Uri(apiAddress) };
            HttpResponseMessage response = await client.PostAsJsonAsync(path, Phone);
            response.EnsureSuccessStatusCode();
            return RedirectToPage("./Index");
        }
    }
}
