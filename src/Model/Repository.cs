using System.Text.Json.Serialization;

namespace ghbin.Model {
    public class Repository {
        public long Id { get; set; }
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        public string Url { get; set; }
    }
}