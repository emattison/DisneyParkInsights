using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace DisneyParkInsights.TableEntities
{
    public class AttractionInfoEntity : TableEntity
    {
        public string Name { get; set; }

        public bool SingleRider { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool PregnantFriendly { get; set; }

        public bool ChildSwap { get; set; }

        public bool WetRide { get; set; }

        public bool RidePhoto { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
