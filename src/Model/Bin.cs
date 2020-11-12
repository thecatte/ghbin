using System.Text.Json.Serialization;

namespace ghbin.Model
{
    public class Bin
    {
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        public string Tag { get; set; }
    }
}