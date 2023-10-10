using DisneyParkInsights.TableEntities;
using DisneyWorldWaitTracker.Data;
using System;

namespace DisneyParkInsights
{
    public static class EntityExtensions
    {
        public static AttractionInfoEntity ToAttractionInfoEntity(this AttractionData attractionData)
        {
            return new AttractionInfoEntity
            {
                PartitionKey = attractionData.Id,
                RowKey = attractionData.Id,
                Name = attractionData.Name,
                Latitude = attractionData.Meta.Latitude,
                Longitude = attractionData.Meta.Longitude,
                ChildSwap = attractionData.Meta.ChildSwap ?? false,
                PregnantFriendly = !attractionData.Meta.UnsuitableForPregnantPeople ?? false,
                RidePhoto = attractionData.Meta.OnRidePhoto ?? false,
                SingleRider = attractionData.Meta.SingleRider ?? false,
                WetRide = attractionData.Meta.MayGetWet ?? false
            };
        }

        public static AttractionWaitTimeEntity ToAttractionWaitTimeEntity(this AttractionData attractionData, ParkConfig parkConfig)
        {
            return new AttractionWaitTimeEntity
            {
                PartitionKey = attractionData.Id,
                RowKey = Guid.NewGuid().ToString(),
                Name = attractionData.Name,
                Status = (int)attractionData.Status.Value,
                LastUpdate = attractionData.LastUpdate.Value,
                TimeZoneOffset = parkConfig.TimeZone.GetUtcOffset(DateTime.UtcNow).TotalHours,
                WaitTimeMinutes = attractionData.WaitTime.Value,
                RetrievalTime = DateTimeOffset.UtcNow
            };
        }
    }
}
