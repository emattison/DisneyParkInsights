using Azure;
using Azure.Data.Tables;
using System;

namespace DisneyWorldWaitTracker.TableEntities
{
    public class AttractionWaitTimeEntity : ITableEntity
    {
        public string Name { get; set; }
        public int WaitTimeMinutes { get; set; }
        public int Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public double TimeZoneOffset { get; set; }
        public DateTimeOffset RetrievalTime { get; set; }
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
