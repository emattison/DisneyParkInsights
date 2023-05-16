using System;
using System.Text.Json.Serialization;

namespace DisneyWorldWaitTracker.Data
{
    public class ParkCalendarEntryData
    {
        [JsonPropertyName("date")]
        public DateTimeOffset Date { get; set; }

        [JsonPropertyName("type")]
        public ParkStatus Status { get; set; }

        [JsonPropertyName("closingTime")]
        public DateTimeOffset ClosingTime { get; set; }

        [JsonPropertyName("openingTime")]
        public DateTimeOffset OpeningTime { get; set; }
    }
}
