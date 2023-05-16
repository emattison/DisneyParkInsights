using DisneyParkInsights.TableEntities;
using DisneyWorldWaitTracker.Data;
using DisneyWorldWaitTracker.TableEntities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;

namespace DisneyParkInsights
{
    public class AttractionInfoAzureTableStorageService : IAttractionInfoStorageService
    {
        private readonly Dictionary<string, TableClient> _azureCloudTableReferences;
        private readonly AttractionInfoAzureTableStorageConfig _config;
        private readonly ILogger<AttractionInfoAzureTableStorageService> _logger;

        public AttractionInfoAzureTableStorageService(IOptions<AttractionInfoAzureTableStorageConfig> config, ILogger<AttractionInfoAzureTableStorageService> logger)
        {
            _azureCloudTableReferences = new Dictionary<string, TableClient>();
            _config = config.Value;
            _logger = logger;
        }

        public async Task StoreAttractionInfo(ParkConfig park, AttractionData attractionInfo)
        {
            var attractionTableClient = await GetCloudTable($"{park.ParkName}Attractions");
            var response = await attractionTableClient.GetEntityIfExistsAsync<AttractionInfoEntity>(attractionInfo.Id, attractionInfo.Id);
            var attractionEntity = response.HasValue ? response.Value : null;
            
            if (attractionEntity is null || DateTime.UtcNow - attractionEntity.Timestamp > TimeSpan.FromHours(24))
            {            
                _logger.LogInformation($"Storing attraction info for [{attractionInfo.Name}] at [{park.ParkName}]");

                var attractionInfoEntity = new AttractionInfoEntity
                {
                    PartitionKey = attractionInfo.Id,
                    RowKey = attractionInfo.Id,
                    Name = attractionInfo.Name,
                    Latitude = attractionInfo.Meta.Latitude,
                    Longitude = attractionInfo.Meta.Longitude,
                    ChildSwap = attractionInfo.Meta.ChildSwap ?? false,
                    PregnantFriendly = !attractionInfo.Meta.UnsuitableForPregnantPeople ?? false,
                    RidePhoto = attractionInfo.Meta.OnRidePhoto ?? false,
                    SingleRider = attractionInfo.Meta.SingleRider ?? false,
                    WetRide = attractionInfo.Meta.MayGetWet ?? false
                };

                var upsertResponse = await attractionTableClient.UpsertEntityAsync(attractionInfoEntity);
            }
            else
            {
                _logger.LogInformation($"Skipped attraction info update for {attractionInfo.Name} due to it being less than 24 hours.");
            }

            if (attractionInfo.Status.HasValue && attractionInfo.LastUpdate.HasValue && attractionInfo.WaitTime.HasValue)
            {
                var waitTimeTableClient = await GetCloudTable($"{park.ParkName}WaitTimes");

                _logger.LogInformation($"Storing wait time for [{attractionInfo.Name}] at [{park.ParkName}]");

                var waitTimeEntity = new AttractionWaitTimeEntity
                {
                    PartitionKey = attractionInfo.Id,
                    RowKey = Guid.NewGuid().ToString(),
                    Name = attractionInfo.Name,
                    Status = (int)attractionInfo.Status.Value,
                    LastUpdate = attractionInfo.LastUpdate.Value,
                    TimeZoneOffset = park.TimeZone.GetUtcOffset(DateTime.UtcNow).TotalHours,
                    WaitTimeMinutes = attractionInfo.WaitTime.Value,
                    RetrievalTime = DateTimeOffset.UtcNow
                };

                await waitTimeTableClient.UpsertEntityAsync(waitTimeEntity);

                _logger.LogInformation(waitTimeEntity.ToString());
            }
            else
            {
                _logger.LogInformation($"Skipped entry for {attractionInfo.Name} due to missing values.");
            }
        }

        private async Task<TableClient> GetCloudTable(string park)
        {
            TableClient tableClient;

            if (_azureCloudTableReferences.TryGetValue(park, out tableClient) == false)
            {
                try
                {
                    tableClient = new TableClient(_config.ConnectionString, park);
                    await tableClient.CreateIfNotExistsAsync();
                    _azureCloudTableReferences.Add(park, tableClient);

                    _logger.LogInformation($"Created azure storage table for {park}");
                }
                catch (Exception excep)
                {
                    _logger.LogError(excep, $"Failed to create azure storage table ({park}).");
                }
            }

            return tableClient;
        }
    }
}
