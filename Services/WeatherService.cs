using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MauiDemo2.Models;
using Microsoft.Maui.Devices.Sensors;

namespace MauiDemo2.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";

        public async Task<WeatherCurrent> GetWeatherAsync()
        {
            var location = await RequestLocationAsync();
            if (location == null)
                throw new InvalidOperationException("Не удалось получить текущую геопозицию.");

            var url = $"{BaseUrl}?latitude={location.Latitude}&longitude={location.Longitude}&current=temperature_2m,relative_humidity_2m";
            System.Diagnostics.Debug.WriteLine($"Weather API Request URL: {url}");

            var response = await _httpClient.GetStringAsync(url);
            System.Diagnostics.Debug.WriteLine($"Weather API Response: {response}");

            var weatherData = JsonSerializer.Deserialize<WeatherResponse>(response);

            if (weatherData?.Current == null)
            {
                System.Diagnostics.Debug.WriteLine("Ошибка: данные о погоде отсутствуют или не распознаны.");
                return new WeatherCurrent();
            }

            return new WeatherCurrent
            {
                Temperature_2m = Math.Round(weatherData.Current.Temperature2m, 1),
                Relative_humidity_2m = Math.Round(weatherData.Current.RelativeHumidity2m, 1)
            };
        }


        private async Task<Location?> RequestLocationAsync()
        {
            return await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Best,
                Timeout = TimeSpan.FromSeconds(10)
            });
        }

    }

    public class WeatherResponse
    {
        [JsonPropertyName("current")]
        public WeatherCurrentData Current { get; set; }
    }

    public class WeatherCurrentData
    {
        [JsonPropertyName("temperature_2m")]
        public double Temperature2m { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public double RelativeHumidity2m { get; set; }
    }

}
