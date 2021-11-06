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

        private string[] Parks = new string[]
        {
            "WaltDisneyWorldMagicKingdom",
            "WaltDisneyWorldHollywoodStudios",
            "WaltDisneyWorldAnimalKingdom",
            "WaltDisneyWorldEpcot",
            "DisneylandResortMagicKingdom",
            "DisneylandResortCaliforniaAdventure"
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
            foreach (string park in Parks)
            {
                log.LogInformation($"Getting calendar for [{park}]");
                IEnumerable<ParkCalendarEntryData> parkCalendarEntries = await _themeParksWiki.GetParkCalendar(park);

                var parkCalendar = parkCalendarEntries.FirstOrDefault(x => x.Date.Date == DateTime.Today);

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

                if (parkCalendar != null
                    && parkCalendar.OpeningTime.UtcDateTime <= DateTime.UtcNow && parkCalendar.ClosingTime.UtcDateTime >= DateTime.UtcNow)
                {
                    log.LogInformation($"Getting wait times for [{park}]");
                    IEnumerable<AttractionData> attractionInfos = await _themeParksWiki.GetParkWaitTimes(park);

                    await AddWaitTimesToStorage(park, attractionInfos);
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
