using MauiDemo2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiDemo2.Services
{
    public class UserService
    {
        private List<User> _users;

        public UserService()
        {
            _users = GenerateUsers();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return _users;
        }

        private List<User> GenerateUsers()
        {
            List<User> users = new List<User>();
            users.Add(new User
            {
                Id = 1,
                Username = "mountain_hiker",
                FirstName = "Иван",
                LastName = "Сидоров",
                SecondName = "Алексеевич"
            });

            users.Add(new User
            {
                Id = 2,
                Username = "forest_explorer",
                FirstName = "Ольга",
                LastName = "Иванова",
                SecondName = "Дмитриевна"
            });

           users.Add(new User
            {
                Id = 3,
                Username = "mountain_forester",
                FirstName = "Гамболл",
                LastName = "Иванова",
                SecondName = "Дмитриевна"
            });

            return users;
        }

        public async Task<UserProfileResponseDto?> GetUserProfileByIdAsync(int? userId, string token)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            System.Diagnostics.Debug.WriteLine($"[USER_SERVICE] Searching for user with id {userId}");
            var response = await client.GetAsync($"http://10.0.2.2:5246/api/users/{userId}/profile");
            System.Diagnostics.Debug.WriteLine($"[USER_SERVICE] Response for user {response}");
            if (!response.IsSuccessStatusCode)
                return null;
            var json = await response.Content.ReadAsStringAsync();
            var userProfile = JsonSerializer.Deserialize<UserProfileResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return userProfile;
        }
    }
}
