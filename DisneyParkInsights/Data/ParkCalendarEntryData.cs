using Newtonsoft.Json;
using System;

namespace DisneyWorldWaitTracker.Data
{
    public class ParkCalendarEntryData
    {
        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("type")]
        public ParkStatus Status { get; set; }

        [JsonProperty("closingTime")]
        public DateTimeOffset ClosingTime { get; set; }

        [JsonProperty("openingTime")]
        public DateTimeOffset OpeningTime { get; set; }
    }
}
