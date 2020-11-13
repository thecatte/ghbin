using System.Text.Json.Serialization;

namespace ghbin.Model
{
    public class Bin
    {
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        [JsonPropertyName("tag")]
        public string Tag { get; set; }
    }
}