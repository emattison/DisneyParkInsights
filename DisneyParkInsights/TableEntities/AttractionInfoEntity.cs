using Azure;
using Azure.Data.Tables;
using System;

namespace DisneyParkInsights.TableEntities
{
    public class AttractionInfoEntity : ITableEntity
    {
        public string Name { get; set; }

        public bool SingleRider { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool PregnantFriendly { get; set; }

        public bool ChildSwap { get; set; }

        public bool WetRide { get; set; }

        public bool RidePhoto { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
