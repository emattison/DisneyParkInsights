using System.Text.Json.Serialization;

namespace DisneyWorldWaitTracker.Data
{
    public class AttractionMetaData
    {
        [JsonPropertyName("singleRider")]
        public bool? SingleRider { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("type")]
        public AttractionType Type { get; set; }

        [JsonPropertyName("unsuitableForPregnantPeople")]
        public bool? UnsuitableForPregnantPeople { get; set; }

        [JsonPropertyName("childSwap")]
        public bool? ChildSwap { get; set; }

        [JsonPropertyName("mayGetWet")]
        public bool? MayGetWet { get; set; }

        [JsonPropertyName("onRidePhoto")]
        public bool? OnRidePhoto { get; set; }
    }
}
