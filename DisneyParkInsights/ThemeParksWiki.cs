using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace DisneyWorldWaitTracker
{
    public interface IThemeParksWiki
    {
        [Get("/preview/parks/WaltDisneyWorldHollywoodStudios/waittime")]
        Task<IEnumerable<AttractionInfo>> GetWaltDisneyWorldHollywoodStudiosWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldAnimalKingdom/waittime")]
        Task<IEnumerable<AttractionInfo>> GetWaltDisneyWorldAnimalKingdomWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldEpcot/waittime")]
        Task<IEnumerable<AttractionInfo>> GetWaltDisneyWorldEpcotWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldMagicKingdom/waittime")]
        Task<IEnumerable<AttractionInfo>> GetWaltDisneyWorldMagicKingdomWaitTimes();

        [Get("/preview/parks")]
        Task<IEnumerable<string>> GetParks();
        
        [Get("/preview/parks/{parkID}")]
        Task<ParkData> GetPark(string parkID);

        [Get("/preview/parks/{parkID}/calendar")]
        Task<IEnumerable<AttractionCalendarEntry>> GetParkCalendar(string parkID);

        [Get("/preview/parks/{parkID}/waittime")]
        Task<IEnumerable<AttractionInfo>> GetParkWaitTimes(string parkID);
    }
}
