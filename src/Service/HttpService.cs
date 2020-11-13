using System;
using System.Net.Http;
using System.Text.Json;

namespace ghbin.Service
{
    public static class HttpService
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions{
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
        
        public static HttpClient GetClient() {
            string cred = ""; // Put your Github username here.
            string base64 = ToBase64(cred);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            client.DefaultRequestHeaders.Add("User-Agent", "GHbin");
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {cred}");
            return client;
        }

        private static string ToBase64(string text) {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(text));
        }
    }
}