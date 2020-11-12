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
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            client.DefaultRequestHeaders.Add("User-Agent", "GHbin");
            return client;
        }
    }
}