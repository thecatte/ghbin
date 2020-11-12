using System;
using System.Text.Json.Serialization;

namespace ghbin.Model
{
    public class Asset
    {
        public long Id { get; set; }
        public string Name { get;set; }
        public string Url { get; set; }
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }
        public long Size { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }
}
