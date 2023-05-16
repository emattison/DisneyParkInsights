using DisneyWorldWaitTracker;
using DisneyWorldWaitTracker.Data;
using DisneyWorldWaitTracker.TableEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyParkInsights.DebugHelper
{
    public class DebugThemeParksWiki : IThemeParksWiki
    {
        private Dictionary<string, ParkData> _parks;
        private Dictionary<string, IEnumerable<ParkCalendarEntryData>> _parkCalendars;
        private Dictionary<string, Dictionary<string, AttractionData>> _parkWaitTimes;

        public DebugThemeParksWiki()
        {
            _parks = new Dictionary<string, ParkData>();
            _parkCalendars = new Dictionary<string, IEnumerable<ParkCalendarEntryData>>();
            _parkWaitTimes = new Dictionary<string, Dictionary<string, AttractionData>>();

            LoadData();
        }

        private void LoadData()
        {
            _parks = new Dictionary<string, ParkData>
            {
                
            };
        }

        public Task<ParkData> GetPark(string parkID)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ParkCalendarEntryData>> GetParkCalendar(string parkID)
        {
            return Task.FromResult(_parkCalendars[parkID]);
        }

        public Task<IEnumerable<string>> GetParks()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AttractionData>> GetParkWaitTimes(string parkID)
        {
            return Task.FromResult((IEnumerable<AttractionData>)_parkWaitTimes[parkID].Values.ToList());
        }

        public Task<IEnumerable<AttractionData>> GetWaltDisneyWorldAnimalKingdomWaitTimes()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AttractionData>> GetWaltDisneyWorldEpcotWaitTimes()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AttractionData>> GetWaltDisneyWorldHollywoodStudiosWaitTimes()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<AttractionData>> GetWaltDisneyWorldMagicKingdomWaitTimes()
        {
            throw new System.NotImplementedException();
        }
    }
}