using Newtonsoft.Json;

namespace DisneyWorldWaitTracker.Data
{
    public class AttractionMetaData
    {
        [JsonProperty("singleRider")]
        public bool? SingleRider { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("type")]
        public AttractionType Type { get; set; }

        [JsonProperty("unsuitableForPregnantPeople")]
        public bool? UnsuitableForPregnantPeople { get; set; }

        [JsonProperty("childSwap")]
        public bool? ChildSwap { get; set; }

        [JsonProperty("mayGetWet")]
        public bool? MayGetWet { get; set; }

        [JsonProperty("onRidePhoto")]
        public bool? OnRidePhoto { get; set; }
    }
}
