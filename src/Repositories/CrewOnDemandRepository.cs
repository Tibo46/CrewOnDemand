using CrewOnDemand.Database;
using CrewOnDemand.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrewOnDemand.Repositories
{
    public class CrewOnDemandRepository: ICrewOnDemandRepository
    {
        private readonly CrewOnDemandContext _db;

        public CrewOnDemandRepository(CrewOnDemandContext db)
        {
            _db = db;
        }

        public async Task<Tuple<int, Booking>> BookAsync(DateTime departureDateTime, DateTime returnDateTime, int pilotId)
        {
            var pilotsQuery = AvailabilityQuery(departureDateTime, returnDateTime);
            var isPilotAvailable = await pilotsQuery.AnyAsync(x => x.Id == pilotId);

            if(!isPilotAvailable)
            {
                return new Tuple<int, Booking>(0, null);
            }

            var newBooking = new Booking
            {
                PilotId = pilotId,
                DepartureDateTime = departureDateTime,
                ReturnDateTime = returnDateTime
            };

            _db.Bookings.Add(newBooking);
            var itemsSaved = await _db.SaveChangesAsync();

            return new Tuple<int, Booking>(itemsSaved, newBooking);
        }

        public async Task<Pilot> GetAvailablePilotAsync(DateTime departureDateTime, DateTime returnDateTime, string location)
        {
            location = location.ToLowerInvariant();
            var pilotsQuery = AvailabilityQuery(departureDateTime, returnDateTime);
            var availablePilot = await pilotsQuery.Where(x => x.Base.Name == location)
                .OrderBy(x => x.Bookings.Count) // Pilot with the least bookings will be returned first, this insure requests are distributed evenly 
                .FirstOrDefaultAsync();

            return availablePilot;
        }

        private IQueryable<Pilot> AvailabilityQuery(DateTime departureDateTime, DateTime returnDateTime)
        {
            var departureDay = (int)departureDateTime.DayOfWeek;
            var arrivalDay = (int)returnDateTime.DayOfWeek;

            return _db.Pilots.Where(x =>
                x.WorkDays.Any(c => c.DayOfTheWeek == departureDay)
                && x.WorkDays.Any(c => c.DayOfTheWeek == arrivalDay)
                && x.Bookings.All(r => r.ReturnDateTime <= departureDateTime
                                    || r.DepartureDateTime >= returnDateTime));
        }
    }
}
