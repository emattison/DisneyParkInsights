using System;
using System.Collections.Generic;
using System.Text;

namespace DisneyWorldWaitTracker
{
    public class AttractionWaitTime
    {
        public string Id { get; set; }
        public int? WaitTime { get; set; }
        public AttractionStatus? Status { get; set; }
        public bool Active { get; set; }
        public DateTimeOffset? LastUpdate { get; set; }
        public MetaData Meta { get; set; }
        public string Name { get; set; }
        public bool FastPass { get; set; }
    }

    public enum AttractionStatus
    {
        Operating,
        Closed,
        Refurbishment,
        Down
    }


    public class MetaData
    {
        public bool SingleRider { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public AttractionType Type {get;set; }
        public bool UnsuitableForPregnantPeople { get; set; }
        public bool ChildSwap { get; set; }
        public bool MayGetWet { get; set; }
        public bool OnRidePhoto { get; set; }
    }
}
