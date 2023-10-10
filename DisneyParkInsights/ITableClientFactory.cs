using Azure.Data.Tables;
using System.Threading.Tasks;

namespace DisneyParkInsights
{
    public interface ITableClientFactory
    {
        Task<TableClient> GetCloudTable(string park);
    }
}