using DisneyWorldWaitTracker.Data;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace DisneyWorldWaitTracker.TableEntities
{
    public class AttractionWaitTimeEntity : TableEntity
    {
        public string Name { get; set; }
        public int WaitTimeMinutes { get; set; }
        public int Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
        public double TimeZoneOffset { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
