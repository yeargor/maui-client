using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiDemo2.Services
{
    public static class RouteServiceCall<T>
    {
        private static readonly string BASE_URL = "http://localhost:3000/";
        static HttpClient Client = new HttpClient() { Timeout = TimeSpan.FromSeconds(60) };

        public static async Task Get(string endPoint, Action<T> onSuccess, Action<Exception> onError)
        {
            try
            {
                string url = BASE_URL + endPoint;
                var response = await Client.GetAsync(url);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(jsonData))
                    {
                        T responseObject = JsonConvert.DeserializeObject<T>(jsonData);
                        onSuccess?.Invoke(responseObject);
                    }
                    else
                    {
                        Exception exception = new Exception("Resource Not Found");
                        onError?.Invoke(exception);
                    }
                }
                else
                {
                    Exception exception = new Exception("Request failed with status code " + response.StatusCode);
                    onError?.Invoke(exception);
                }
            }
            catch (Exception exception)
            {
                onError?.Invoke(exception);
            }
        }
    }
}
