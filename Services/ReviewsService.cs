using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MauiDemo2.Models;

namespace MauiDemo2.Services
{
    public class ReviewsService
    {
        private readonly HttpClient _httpClient;
        public ReviewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CreateReviewAsync(CreateReviewRequestDto dto)
        {
            var json = JsonSerializer.Serialize(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://byways-p378.onrender.com/api/reviews/create", content);
            return response.IsSuccessStatusCode;
        }
    }
}
