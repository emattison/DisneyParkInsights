using DisneyWorldWaitTracker;
using DisneyWorldWaitTracker.Data;
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
                { "DisneylandResortMagicKingdom", new ParkData
                    {
                        Id = "DisneylandResortMagicKingdom",
                        Name = "Disneyland Park",
                        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")
                    }
                },
                { "DisneylandResortCaliforniaAdventure", new ParkData
                    {
                        Id = "DisneylandResortCaliforniaAdventure",
                        Name = "Disney California Adventure Park",
                        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time")
                    }
                }
            };

            _parkCalendars = new Dictionary<string, IEnumerable<ParkCalendarEntryData>>
            {
                { "DisneylandResortMagicKingdom", new List<ParkCalendarEntryData>
                    {
                        new ParkCalendarEntryData
                        {
                            OpeningTime = DateTime.Now.AddHours(-2),
                            ClosingTime = DateTime.Now.AddHours(2),
                            Date = DateTime.Today,
                            Status = ParkStatus.Operating
                        }
                    }
                },
                { "DisneylandResortCaliforniaAdventure", new List<ParkCalendarEntryData>
                    {
                        new ParkCalendarEntryData
                        {
                            OpeningTime = DateTime.Now.AddHours(-2),
                            ClosingTime = DateTime.Now.AddHours(2),
                            Date = DateTime.Today,
                            Status = ParkStatus.Operating
                        }
                    }
                }
            };

            _parkWaitTimes = new Dictionary<string, Dictionary<string, AttractionData>>
            {
                { "DisneylandResortMagicKingdom", new Dictionary<string, AttractionData>
                    {
                        { "DisneylandResortMagicKingdom_1", new AttractionData
                            {
                                Id = "DisneylandResortMagicKingdom_1",
                                Name = "Space Mountain Water Slide",
                                Active = true,
                                FastPass = true,
                                Status = AttractionStatus.Operating,
                                WaitTime = 15,
                                LastUpdate = DateTime.Now,
                                Meta = new AttractionMetaData
                                {
                                    ChildSwap = true,
                                    MayGetWet = true,
                                    OnRidePhoto = true,
                                    SingleRider = true,
                                    Type = AttractionType.Attraction,
                                    UnsuitableForPregnantPeople = true,
                                    Latitude = 100,
                                    Longitude = 100
                                }
                            }
                        },
                        { "DisneylandResortMagicKingdom_2", new AttractionData
                            {
                                Id = "DisneylandResortMagicKingdom_2",
                                Name = "Thunder Blunder Railroad",
                                Active = false,
                                FastPass = true,
                                Status = AttractionStatus.Operating,
                                WaitTime = null,
                                LastUpdate = DateTime.Now,
                                Meta = new AttractionMetaData
                                {
                                    ChildSwap = true,
                                    MayGetWet = false,
                                    OnRidePhoto = true,
                                    SingleRider = true,
                                    Type = AttractionType.Attraction,
                                    UnsuitableForPregnantPeople = true,
                                    Latitude = 200,
                                    Longitude = 200
                                }
                            }
                        }
                    }
                },
                { "DisneylandResortCaliforniaAdventure", new Dictionary<string, AttractionData>
                    {
                        { "DisneylandResortCaliforniaAdventure_1", new AttractionData
                            {
                                Id = "DisneylandResortCaliforniaAdventure_1",
                                Name = "Guardians of the Hotel Tower",
                                Active = true,
                                FastPass = true,
                                Status = AttractionStatus.Operating,
                                WaitTime = 60,
                                LastUpdate = DateTime.Now,
                                Meta = new AttractionMetaData
                                {
                                    ChildSwap = true,
                                    MayGetWet = true,
                                    OnRidePhoto = true,
                                    SingleRider = true,
                                    Type = AttractionType.Attraction,
                                    UnsuitableForPregnantPeople = true,
                                    Latitude = 100,
                                    Longitude = 100
                                }
                            }
                        },
                        { "DisneylandResortCaliforniaAdventure_2", new AttractionData
                            {
                                Id = "DisneylandResortCaliforniaAdventure_2",
                                Name = "Cars Racing through a Canyon",
                                Active = false,
                                FastPass = true,
                                Status = AttractionStatus.Operating,
                                WaitTime = null,
                                LastUpdate = DateTime.Now,
                                Meta = new AttractionMetaData
                                {
                                    ChildSwap = true,
                                    MayGetWet = false,
                                    OnRidePhoto = true,
                                    SingleRider = true,
                                    Type = AttractionType.Attraction,
                                    UnsuitableForPregnantPeople = true,
                                    Latitude = 200,
                                    Longitude = 200
                                }
                            }
                        }
                    }
                }
            };
        }

        public Task<ParkData> GetPark(string parkID)
        {
            if (_parks.ContainsKey(parkID))
                return Task.FromResult(_parks[parkID]);
            else
                return Task.FromResult<ParkData>(null);
        }

        public Task<IEnumerable<ParkCalendarEntryData>> GetParkCalendar(string parkID)
        {
            if (_parkCalendars.ContainsKey(parkID))
                return Task.FromResult(_parkCalendars[parkID]);
            else
                return Task.FromResult<IEnumerable<ParkCalendarEntryData>>(null);
        }

        public Task<IEnumerable<string>> GetParks()
        {
            return Task.FromResult<IEnumerable<string>>(_parks.Keys.ToList());
        }

        public Task<IEnumerable<AttractionData>> GetParkWaitTimes(string parkID)
        {
            if (_parkWaitTimes.ContainsKey(parkID))
                return Task.FromResult((IEnumerable<AttractionData>)_parkWaitTimes[parkID].Values.ToList());
            else
                return Task.FromResult<IEnumerable<AttractionData>>(null);
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