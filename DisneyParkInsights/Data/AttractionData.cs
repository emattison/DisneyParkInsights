using Newtonsoft.Json;
using System;

namespace DisneyWorldWaitTracker.Data
{
    public class AttractionData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("waitTime")]
        public int? WaitTime { get; set; }

        [JsonProperty("status")]
        public AttractionStatus? Status { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTimeOffset? LastUpdate { get; set; }

        [JsonProperty("meta")]
        public AttractionMetaData Meta { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fastPass")]
        public bool FastPass { get; set; }
    }
}
