using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ghbin.Model
{
    public class Configuration
    {
        [JsonPropertyName("bins")]
        public List<Bin> Bins { get;set; }

        public Configuration() {
            Bins = new List<Bin>();
        }
    }
}