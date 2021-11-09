using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisneyParkInsights;
using DisneyWorldWaitTracker.Data;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DisneyWorldWaitTracker
{
    public class Functions
    {
        private readonly IThemeParksWiki _themeParksWiki;
        private readonly IAttractionInfoStorageService _attractionInfoStorage;

        private ParkConfig[] Parks = new ParkConfig[]
        {
            new ParkConfig("WaltDisneyWorldMagicKingdom", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("WaltDisneyWorldHollywoodStudios", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("WaltDisneyWorldAnimalKingdom", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("WaltDisneyWorldEpcot", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("DisneylandResortMagicKingdom", TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")),
            new ParkConfig("DisneylandResortCaliforniaAdventure", TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
        };

        public Functions(IThemeParksWiki themeParksWiki, IAttractionInfoStorageService attractionInfoStorage)
        {
            _themeParksWiki = themeParksWiki;
            _attractionInfoStorage = attractionInfoStorage;
        }

        [FunctionName("TriggerAttractionDataRetrieval")]
        public async Task TriggerAttractionDataRetrieval([TimerTrigger("%AttractionRetrievalInterval%")] TimerInfo myTimer, ILogger log)
        {
            //Get park data
            foreach (ParkConfig park in Parks)
            {
                log.LogInformation($"Getting calendar for [{park.ParkName}]");
                IEnumerable<ParkCalendarEntryData> parkCalendarEntries = await _themeParksWiki.GetParkCalendar(park.ParkName);

                var currentTimeAtPark = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, park.TimeZone);
                var parkCalendar = parkCalendarEntries.FirstOrDefault(x => x.Date.Date == currentTimeAtPark.Date);

#if DEBUG
                if (parkCalendar == null)
                {
                    parkCalendar = new ParkCalendarEntryData
                    {
                        Date = DateTime.Now,
                        OpeningTime = DateTime.UtcNow.AddHours(-1),
                        ClosingTime = DateTime.UtcNow.AddHours(1),
                        Status = ParkStatus.Operating
                    };
                }
#endif
                if (parkCalendar == null)
                {
                    log.LogWarning($"[{park.ParkName}] missing calendar for {currentTimeAtPark.Date}");
                    return;
                }

                if (parkCalendar.OpeningTime <= currentTimeAtPark && parkCalendar.ClosingTime >= currentTimeAtPark)
                {
                    log.LogInformation($"Getting wait times for [{park.ParkName}]");
                    IEnumerable<AttractionData> attractionInfos = await _themeParksWiki.GetParkWaitTimes(park.ParkName);

                    await AddWaitTimesToStorage(park.ParkName, attractionInfos);
                }
                else
                {
                    log.LogInformation($"Now: {currentTimeAtPark} [{park.ParkName}] closed. Hours: {parkCalendar.OpeningTime} - {parkCalendar.ClosingTime}");
                }
            }
        }

        private async Task AddWaitTimesToStorage(string park, IEnumerable<AttractionData> attractionWaitTimes)
        {
            foreach (AttractionData attractionWaitTime in attractionWaitTimes)
            {
                if (attractionWaitTime.Meta.Type == AttractionType.Attraction
                    && attractionWaitTime.WaitTime.HasValue
                    && attractionWaitTime.Status.HasValue
                    && (attractionWaitTime.Status == AttractionStatus.Operating || attractionWaitTime.Status == AttractionStatus.Down))
                {
                    await _attractionInfoStorage.StoreAttractionInfo(park, attractionWaitTime);
                }
            }
        }
    }
}
