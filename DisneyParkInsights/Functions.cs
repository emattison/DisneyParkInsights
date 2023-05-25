using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisneyParkInsights;
using DisneyWorldWaitTracker.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DisneyWorldWaitTracker
{
    public class Functions
    {
        private readonly IThemeParksWiki _themeParksWiki;
        private readonly IAttractionInfoStorageService _attractionInfoStorage;
        private readonly ILogger<Functions> _logger;
        private ParkConfig[] Parks = new ParkConfig[]
        {
            new ParkConfig("WaltDisneyWorldMagicKingdom", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("WaltDisneyWorldHollywoodStudios", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("WaltDisneyWorldAnimalKingdom", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("WaltDisneyWorldEpcot", TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")),
            new ParkConfig("DisneylandResortMagicKingdom", TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")),
            new ParkConfig("DisneylandResortCaliforniaAdventure", TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"))
        };

        public Functions(IThemeParksWiki themeParksWiki, IAttractionInfoStorageService attractionInfoStorage, ILogger<Functions> logger)
        {
            _themeParksWiki = themeParksWiki;
            _attractionInfoStorage = attractionInfoStorage;
            this._logger = logger;
        }

        [Function("TriggerAttractionDataRetrieval")]
        public async Task TriggerAttractionDataRetrieval([TimerTrigger("0 */15 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            //Get park data
            foreach (ParkConfig park in Parks)
            {
                _logger.LogInformation($"Getting calendar for [{park.ParkName}]");
                IEnumerable<ParkCalendarEntryData> parkCalendarEntries = await _themeParksWiki.GetParkCalendar(park.ParkName);


                if (parkCalendarEntries == null)
                {
                    _logger.LogWarning($"[{park.ParkName}] missing calendar entries.");
                    continue;
                }

                var currentTimeAtPark = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, park.TimeZone);
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
                    _logger.LogWarning($"[{park.ParkName}] missing calendar for {currentTimeAtPark.Date}");
                    continue;
                }

                if (parkCalendar.OpeningTime <= currentTimeAtPark && parkCalendar.ClosingTime >= currentTimeAtPark)
                {
                    _logger.LogInformation($"Getting wait times for [{park.ParkName}]");
                    IEnumerable<AttractionData> attractionInfos = await _themeParksWiki.GetParkWaitTimes(park.ParkName);

                    if (attractionInfos == null)
                    {
                        _logger.LogWarning($"[{park.ParkName}] missing attraction data for {currentTimeAtPark.Date}");
                        continue;
                    }

                    await AddWaitTimesToStorage(park, attractionInfos);
                }
                else
                {
                    _logger.LogInformation($"Now: {currentTimeAtPark} [{park.ParkName}] closed. Hours: {parkCalendar.OpeningTime} - {parkCalendar.ClosingTime}");
                }
            }
        }

        private async Task AddWaitTimesToStorage(ParkConfig park, IEnumerable<AttractionData> attractionWaitTimes)
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
