using DisneyWorldWaitTracker;
using DisneyWorldWaitTracker.Data;
using System.Text;
using System.Threading.Tasks;

namespace DisneyParkInsights
{
    public interface IAttractionInfoStorageService
    {
        Task StoreAttractionInfo(ParkConfig park, AttractionData attractionInfo);
    }
}
