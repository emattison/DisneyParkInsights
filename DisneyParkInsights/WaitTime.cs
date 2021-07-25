using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DisneyWorldWaitTracker
{
    public class ParkData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public TimeZoneInfo TimeZone { get; set; }
    }

    public class AttractionInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("WaitTiwe")]
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

    public enum AttractionStatus
    {
        Operating,
        Closed,
        Refurbishment,
        Down
    }


    public class AttractionMetaData
    {
        [JsonProperty("singleRider")]
        public bool SingleRider { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        
        [JsonProperty("type")]
        public AttractionType Type {get;set; }
        
        [JsonProperty("unsuitableForPregnantPeople")]
        public bool UnsuitableForPregnantPeople { get; set; }
        
        [JsonProperty("childSwap")]
        public bool ChildSwap { get; set; }
        
        [JsonProperty("mayGetWet")]
        public bool MayGetWet { get; set; }
        
        [JsonProperty("onRidePhoto")]
        public bool OnRidePhoto { get; set; }
    }

    public class AttractionCalendarEntry
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
