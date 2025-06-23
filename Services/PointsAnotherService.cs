using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using MauiDemo2.Models;
using Newtonsoft.Json.Linq;
using Point = MauiDemo2.Models.Point;

public class PointsAnotherService
{
    private static readonly HttpClient client = new HttpClient();
    
    public async Task<JObject> GetRouteAsync(IEnumerable<PlaceInfoResponseDto> points, string apiKey, string mode)
    {
        string waypoints = string.Join("|", points.Select(p => $"{p.LocationInfo.Latitude},{p.LocationInfo.Longitude}"));
        string url = $"https://api.geoapify.com/v1/routing?waypoints={waypoints}&mode={mode}&format=json&apiKey={apiKey}";

        Debug.WriteLine($"api request: {url}");
        HttpResponseMessage response = await client.GetAsync(url);
        string jsonResponse = await response.Content.ReadAsStringAsync();
        
        return JObject.Parse(jsonResponse);
    }
}
