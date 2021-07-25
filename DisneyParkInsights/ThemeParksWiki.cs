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
        Task<IEnumerable<AttractionWaitTime>> GetWaltDisneyWorldHollywoodStudiosWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldAnimalKingdom/waittime")]
        Task<IEnumerable<AttractionWaitTime>> GetWaltDisneyWorldAnimalKingdomWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldEpcot/waittime")]
        Task<IEnumerable<AttractionWaitTime>> GetWaltDisneyWorldEpcotWaitTimes();

        [Get("/preview/parks/WaltDisneyWorldMagicKingdom/waittime")]
        Task<IEnumerable<AttractionWaitTime>> GetWaltDisneyWorldMagicKingdomWaitTimes();

        [Get("/preview/parks")]
        Task<string> GetParks();
        
        [Get("/preview/parks/{parkID}")]
        Task<string> GetPark(string parkID);

        [Get("/preview/parks/{parkID}/calendar")]
        Task<string> GetParkCalendar(string parkID);

        [Get("/preview/parks/{parkID}/waittime")]
        Task<IEnumerable<AttractionWaitTime>> GetParkWaitTimes(string parkID);
    }
}
