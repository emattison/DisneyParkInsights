using System;
using System.Collections.Generic;
using System.Text;

namespace DisneyParkInsights
{
    public struct ParkConfig
    {
        public ParkConfig(string parkName, TimeZoneInfo timeZone)
        {
            ParkName = parkName;
            TimeZone = timeZone;
        }

        public string ParkName { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
    }
}
