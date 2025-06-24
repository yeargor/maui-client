using MauiDemo2.Models;
using MauiDemo2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MauiDemo2.Services
{
    public class MarkService
    {
        private List<Mark> _marks;

        public async Task<List<Mark>> GetMarksAsync()
        {
            return _marks;
        }

        public async Task<List<Mark>> GetMarksByUserIdAsync(int userId)
        {
            var marks = await GetMarksAsync();
            return marks.Where(m => m.UserId == userId).ToList();
        }

        public async Task<List<Mark>> GetMarksByTripIdAsync(int tripId)
        {
            var marks = await GetMarksAsync();
            return marks.Where(m => m.RouteId == tripId).ToList();
        }

        public async Task<bool> CreateMarkAsync(Mark mark)
        {
            var token = JwtService.GetJwtToken();
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("JWT токен не найден");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var json = System.Text.Json.JsonSerializer.Serialize(mark);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://byways-p378.onrender.com/api/marks/create", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMarkAsync(int? markId)
        {
            var token = JwtService.GetJwtToken();
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("JWT токен не найден");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"https://byways-p378.onrender.com/api/marks/delete/{markId}");
            return response.IsSuccessStatusCode;
        }
    }
}
