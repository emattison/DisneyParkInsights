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
        private const string MagicKingdomRunFrequency = "0 */15 13-22 * * *";
        private const string HollywoodStudiosRunFrequency = "0 */15 14-23 * * *";
        private const string EpcotRunFrequency = "0 */15 15-23 * * *";
        private const string AnimalKingdomRunFrequency = "0 */15 13-21 * * *";

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
        public async Task TriggerAttractionDataRetrieval([TimerTrigger("%AttractionRetrievalInterval%")]TimerInfo myTimer, ILogger log)
        {
            //Get park data
            foreach (string park in Parks)
            {
                IEnumerable<ParkCalendarEntryData> parkCalendarEntries = await _themeParksWiki.GetParkCalendar(park);

                var parkCalendar = parkCalendarEntries.FirstOrDefault(x => x.Date.Date == DateTime.Today);

                if (parkCalendar.OpeningTime.UtcDateTime <= DateTime.UtcNow && parkCalendar.ClosingTime.UtcDateTime >= DateTime.UtcNow)
                {
                    IEnumerable<AttractionData> attractionInfos = await _themeParksWiki.GetParkWaitTimes(park);

                    await AddWaitTimesToStorage(park, attractionInfos);
                }
            }
        }


        /* Old Functions
        [FunctionName("MagicKingdomWaitTimes")]
        public async Task GetMagicKingdomWaitTimes([TimerTrigger(MagicKingdomRunFrequency)]TimerInfo myTimer, [Table("MagicKingdomWaitTimes")]CloudTable cloudTable, ILogger log)
        {
            log.LogInformation($"Magic Kingdom wait times pulled at: {DateTime.Now}");

            var attractionWaitTimes = await _themeParksWiki.GetWaltDisneyWorldMagicKingdomWaitTimes();

            await AddWaitTimesToTable(cloudTable, attractionWaitTimes);

            log.LogInformation($"Magic Kingdom wait times finished at: {DateTime.Now}");
        }

        [FunctionName("HollywoodStudiosWaitTimes")]
        public async Task GetHollywoodStudiosWaitTimes([TimerTrigger(HollywoodStudiosRunFrequency)]TimerInfo myTimer, [Table("HollywoodStudiosWaitTimes")]CloudTable cloudTable, ILogger log)
        {
            log.LogInformation($"Hollywood Studios wait times pulled at: {DateTime.Now}");

            var attractionWaitTimes = await _themeParksWiki.GetWaltDisneyWorldHollywoodStudiosWaitTimes();

            await AddWaitTimesToTable(cloudTable, attractionWaitTimes);

            log.LogInformation($"Hollywood Studios wait times finished at: {DateTime.Now}");
        }

        [FunctionName("EpcotWaitTimes")]
        public async Task GetEpcotWaitTimes([TimerTrigger(EpcotRunFrequency)]TimerInfo myTimer, [Table("EpcotWaitTimes")]CloudTable cloudTable, ILogger log)
        {
            log.LogInformation($"Epcot wait times pulled at: {DateTime.Now}");

            var attractionWaitTimes = await _themeParksWiki.GetWaltDisneyWorldEpcotWaitTimes();

            await AddWaitTimesToTable(cloudTable, attractionWaitTimes);

            log.LogInformation($"Epcot wait times finished at: {DateTime.Now}");
        }

        [FunctionName("AnimalKingdomWaitTimes")]
        public async Task GetAnimalKingdomWaitTimes([TimerTrigger(AnimalKingdomRunFrequency)]TimerInfo myTimer, [Table("AnimalKingdomWaitTimes")]CloudTable cloudTable, ILogger log)
        {
            log.LogInformation($"AnimalKingdom wait times pulled at: {DateTime.Now}");

            var attractionWaitTimes = await _themeParksWiki.GetWaltDisneyWorldAnimalKingdomWaitTimes();

            await AddWaitTimesToTable(cloudTable, attractionWaitTimes);

            log.LogInformation($"Animal Kingdom wait times finished at: {DateTime.Now}");
        }
        */

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
