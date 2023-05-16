using System;
using System.Text.Json.Serialization;

namespace DisneyWorldWaitTracker.Data
{
    public class AttractionData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("waitTime")]
        public int? WaitTime { get; set; }

        [JsonPropertyName("status")]
        public AttractionStatus? Status { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("lastUpdate")]
        public DateTimeOffset? LastUpdate { get; set; }

        [JsonPropertyName("meta")]
        public AttractionMetaData Meta { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("fastPass")]
        public bool FastPass { get; set; }
    }
}
