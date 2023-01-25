using DisneyParkInsights.TableEntities;
using DisneyWorldWaitTracker.Data;
using DisneyWorldWaitTracker.TableEntities;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisneyParkInsights
{
    public class AttractionInfoAzureTableStorageService : IAttractionInfoStorageService
    {
        private readonly Dictionary<string, CloudTable> _azureCloudTableReferences;
        private readonly ILogger<AttractionInfoAzureTableStorageService> _logger;
        private CloudTableClient _cloudTableClient;

        public AttractionInfoAzureTableStorageService(IOptions<AttractionInfoAzureTableStorageConfig> config, ILogger<AttractionInfoAzureTableStorageService> logger)
        {
            _azureCloudTableReferences = new Dictionary<string, CloudTable>();

            var cloudStorageAccount = CloudStorageAccount.Parse(config.Value.ConnectionString);
            _cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            _logger = logger;
        }

        public async Task StoreAttractionInfo(ParkConfig park, AttractionData attractionInfo)
        {
            CloudTable attractionCloudTable = await GetCloudTable($"{park.ParkName}Attractions");
            
            var retrieveOperation = await attractionCloudTable.ExecuteAsync(TableOperation.Retrieve<AttractionInfoEntity>(attractionInfo.Id, attractionInfo.Id));
            
            if (retrieveOperation.Entity is null || DateTime.UtcNow - retrieveOperation.Entity.Timestamp.UtcDateTime > TimeSpan.FromHours(24))
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

                await attractionCloudTable.ExecuteAsync(TableOperation.InsertOrReplace(attractionInfoEntity));
            }
            else
            {
                _logger.LogInformation($"Skipped attraction info update for {attractionInfo.Name} due to it being less than 24 hours.");
            }

            if (attractionInfo.Status.HasValue && attractionInfo.LastUpdate.HasValue && attractionInfo.WaitTime.HasValue)
            {
                CloudTable waitTimeCloudTable = await GetCloudTable($"{park.ParkName}WaitTimes");

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

                await waitTimeCloudTable.ExecuteAsync(TableOperation.Insert(waitTimeEntity));

                _logger.LogInformation(waitTimeEntity.ToString());
            }
            else
            {
                _logger.LogInformation($"Skipped entry for {attractionInfo.Name} due to missing values.");
            }
        }

        private async Task<CloudTable> GetCloudTable(string park)
        {
            CloudTable cloudTable;

            if (_azureCloudTableReferences.TryGetValue(park, out cloudTable) == false)
            {
                try
                {
                    cloudTable = _cloudTableClient.GetTableReference(park);
                    await cloudTable.CreateIfNotExistsAsync();
                    _azureCloudTableReferences.Add(park, cloudTable);

                    _logger.LogInformation($"Created azure storage table for {park}");
                }
                catch (Exception excep)
                {
                    _logger.LogError(excep, $"Failed to create azure storage table ({park}).");
                }
            }

            return cloudTable;
        }
    }
}
