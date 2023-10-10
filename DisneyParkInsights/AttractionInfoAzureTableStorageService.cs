using DisneyParkInsights.TableEntities;
using DisneyWorldWaitTracker.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DisneyParkInsights
{
    public class AttractionInfoAzureTableStorageService : IAttractionInfoStorageService
    {
        private readonly ITableClientFactory _tableClientFactory;
        private readonly ILogger<AttractionInfoAzureTableStorageService> _logger;

        public AttractionInfoAzureTableStorageService(ITableClientFactory tableClientFactory, ILogger<AttractionInfoAzureTableStorageService> logger)
        {
            _tableClientFactory = tableClientFactory;
            _logger = logger;
        }

        public async Task StoreAttractionInfo(ParkConfig parkConfig, AttractionData attractionInfo)
        {
            if (!parkConfig.IsValid())
            {
                _logger.LogError(nameof(parkConfig) + " is missing data. ParkName: {parkName}  TimeZone: {timeZone}", parkConfig.ParkName, parkConfig.TimeZone);
                throw new ArgumentException("parkConfig not set properly. See log for details...", nameof(parkConfig));
            }

            if (attractionInfo is null)
            {
                _logger.LogError(nameof(attractionInfo) + "is null when it should not be.");
                throw new ArgumentNullException(nameof(attractionInfo));
            }

            var attractionTableClient = await _tableClientFactory.GetCloudTable($"{parkConfig.ParkName}Attractions");
            var response = await attractionTableClient.GetEntityIfExistsAsync<AttractionInfoEntity>(attractionInfo.Id, attractionInfo.Id);
            var attractionEntity = response.HasValue ? response.Value : null;
            
            if (attractionEntity is null || DateTime.UtcNow - attractionEntity.Timestamp > TimeSpan.FromHours(24))
            {            
                _logger.LogInformation($"Storing attraction info for [{attractionInfo.Name}] at [{parkConfig.ParkName}]");

                var attractionInfoEntity = attractionInfo.ToAttractionInfoEntity();

                _ = await attractionTableClient.UpsertEntityAsync(attractionInfoEntity);
            }
            else
            {
                _logger.LogInformation($"Skipped attraction info update for {attractionInfo.Name} due to it being less than 24 hours.");
            }

            if (attractionInfo.Status.HasValue && attractionInfo.LastUpdate.HasValue && attractionInfo.WaitTime.HasValue)
            {
                var waitTimeTableClient = await _tableClientFactory.GetCloudTable($"{parkConfig.ParkName}WaitTimes");

                _logger.LogInformation($"Storing wait time for [{attractionInfo.Name}] at [{parkConfig.ParkName}]");

                var waitTimeEntity = attractionInfo.ToAttractionWaitTimeEntity(parkConfig);

                _ = await waitTimeTableClient.UpsertEntityAsync(waitTimeEntity);

                _logger.LogInformation(waitTimeEntity.ToString());
            }
            else
            {
                _logger.LogInformation($"Skipped entry for {attractionInfo.Name} due to missing values.");
            }
        }
    }
}
