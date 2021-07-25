using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DisneyWorldWaitTracker
{
    public class Functions
    {
        private readonly IThemeParksWiki _themeParksWiki;
        private const string MagicKingdomRunFrequency = "0 */15 13-22 * * *";
        private const string HollywoodStudiosRunFrequency = "0 */15 14-23 * * *";
        private const string EpcotRunFrequency = "0 */15 15-23 * * *";
        private const string AnimalKingdomRunFrequency = "0 */15 13-21 * * *";

        public Functions(IThemeParksWiki themeParksWiki)
        {
            _themeParksWiki = themeParksWiki;
        }

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

        private static async Task AddWaitTimesToTable(CloudTable cloudTable, IEnumerable<AttractionWaitTime> attractionWaitTimes)
        {
            foreach (AttractionWaitTime attractionWaitTime in attractionWaitTimes)
            {
                if (attractionWaitTime.WaitTime.HasValue
                    && (attractionWaitTime.Status == AttractionStatus.Operating
                        || attractionWaitTime.Status == AttractionStatus.Down)
                    && attractionWaitTime.Status.HasValue)
                {
                    var waitTime = new WaitTime
                    {
                        PartitionKey = attractionWaitTime.Id,
                        RowKey = Guid.NewGuid().ToString(),
                        Name = attractionWaitTime.Name,
                        Status = attractionWaitTime.Status.Value,
                        LastUpdate = attractionWaitTime.LastUpdate.Value,
                        WaitTimeMinutes = attractionWaitTime.WaitTime.Value
                    };

                    await cloudTable.ExecuteAsync(TableOperation.Insert(waitTime));
                }
            }
        }
    }

    public class WaitTime : TableEntity
    {
        public string Name { get; set; }
        public int WaitTimeMinutes { get; set; }
        public AttractionStatus Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }
    }
}
