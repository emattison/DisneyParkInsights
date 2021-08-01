using DisneyParkInsights.TableEntities;
using DisneyWorldWaitTracker.Data;
using DisneyWorldWaitTracker.TableEntities;
using Microsoft.Azure.Cosmos.Table;
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

        public async Task StoreAttractionInfo(string park, AttractionData attractionInfo)
        {
            CloudTable attractionCloudTable = await GetCloudTable($"{park}Attractions");

            var attractionInfoEntity = new AttractionInfoEntity
            {
                PartitionKey = attractionInfo.Id,
                RowKey = attractionInfo.Id,
                Name = attractionInfo.Name,
                Latitude = attractionInfo.Meta.Latitude,
                Longitude = attractionInfo.Meta.Longitude,
                ChildSwap = attractionInfo.Meta.ChildSwap,
                PregnantFriendly = !attractionInfo.Meta.UnsuitableForPregnantPeople,
                RidePhoto = attractionInfo.Meta.OnRidePhoto,
                SingleRider = attractionInfo.Meta.SingleRider,
                WetRide = attractionInfo.Meta.MayGetWet
            };

            await attractionCloudTable.ExecuteAsync(TableOperation.InsertOrReplace(attractionInfoEntity));

            CloudTable waitTimeCloudTable = await GetCloudTable($"{park}WaitTimes");

            _logger.LogInformation(attractionInfoEntity.ToString());

            var waitTimeEntity = new AttractionWaitTimeEntity
            {
                PartitionKey = attractionInfo.Id,
                RowKey = Guid.NewGuid().ToString(),
                Name = attractionInfo.Name,
                Status = attractionInfo.Status.Value,
                LastUpdate = attractionInfo.LastUpdate.Value,
                WaitTimeMinutes = attractionInfo.WaitTime.Value
            };

            await waitTimeCloudTable.ExecuteAsync(TableOperation.Insert(waitTimeEntity));

            _logger.LogInformation(waitTimeEntity.ToString());
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
