using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisneyParkInsights
{
    public class TableClientFactory : ITableClientFactory
    {
        private readonly Dictionary<string, TableClient> _azureCloudTableReferences;
        private readonly AttractionInfoAzureTableStorageConfig _config;
        private readonly ILogger<TableClientFactory> _logger;

        public TableClientFactory(IOptions<AttractionInfoAzureTableStorageConfig> config, ILogger<TableClientFactory> logger)
        {
            _azureCloudTableReferences = new Dictionary<string, TableClient>();
            _config = config.Value;
            _logger = logger;
        }

        public async Task<TableClient> GetCloudTable(string park)
        {
            TableClient tableClient;

            if (_azureCloudTableReferences.TryGetValue(park, out tableClient) == false)
            {
                try
                {
                    tableClient = new TableClient(_config.ConnectionString, park);
                    var response = await tableClient.CreateIfNotExistsAsync();
                    _azureCloudTableReferences.Add(park, tableClient);

                    switch (response.GetRawResponse().Status)
                    {
                        case 200:
                            _logger.LogInformation("Created azure storage table for {park}", park);
                            break;
                        case 409:
                            _logger.LogInformation("Table for {park} already exists.", park);
                            break;
                    }
                }
                catch (Exception excep)
                {
                    _logger.LogError(excep, "Failed to create azure storage table for {park).", park);
                }
            }

            return tableClient;
        }
    }
}
