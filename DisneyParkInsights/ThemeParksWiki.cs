using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DisneyWorldWaitTracker.Data;
using Refit;

namespace DisneyWorldWaitTracker
{
    public interface IThemeParksWiki
    {
        [Get("/preview/parks/WaltDisneyWorldHollywoodStudios/waittime")]
        Task<IEnumerable<AttractionData>> GetWaltDisneyWorldHollywoodStudiosWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldAnimalKingdom/waittime")]
        Task<IEnumerable<AttractionData>> GetWaltDisneyWorldAnimalKingdomWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldEpcot/waittime")]
        Task<IEnumerable<AttractionData>> GetWaltDisneyWorldEpcotWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldMagicKingdom/waittime")]
        Task<IEnumerable<AttractionData>> GetWaltDisneyWorldMagicKingdomWaitTimes();

        [Get("/preview/parks")]
        Task<IEnumerable<string>> GetParks();
        
        [Get("/preview/parks/{parkID}")]
        Task<ParkData> GetPark(string parkID);

        [Get("/preview/parks/{parkID}/calendar")]
        Task<IEnumerable<ParkCalendarEntryData>> GetParkCalendar(string parkID);

        [Get("/preview/parks/{parkID}/waittime")]
        Task<IEnumerable<AttractionData>> GetParkWaitTimes(string parkID);
    }
}
