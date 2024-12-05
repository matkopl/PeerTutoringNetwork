using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PeerTutoringNetwork.DTOs;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace PeerTutoringNetwork.Pages
{
    public class ProfilePageModel : PageModel
    {

        [BindProperty]
        public ProfileDTO Profile { get; set; }

        private readonly HttpClient _httpClient;

        public ProfilePageModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/profile/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            Profile = await response.Content.ReadFromJsonAsync<ProfileDTO>();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/profile/{id}", Profile);
            if (!response.IsSuccessStatusCode) return BadRequest("Failed to update.");

            return RedirectToPage("/Index");
        }
    }
}