using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ghbin.Model
{
    public class Release
    {
        public long Id { get; set; }
        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }
        [JsonPropertyName("tarball_url")]
        public string TarballUrl { get; set; }
        [JsonPropertyName("zipball_url")]
        public string ZipballUrl { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }
        public List<Asset> Assets { get; set; }
    }
}