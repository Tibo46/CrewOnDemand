using CrewOnDemand.Models;
using System;
using System.Threading.Tasks;

namespace CrewOnDemand.Repositories
{
    public interface ICrewOnDemandRepository
    {
        Task<Tuple<int, Booking>> BookAsync(DateTime departureDateTime, DateTime returnDateTime, int pilotId);
        Task<Pilot> GetAvailablePilotAsync(DateTime departureDateTime, DateTime returnDateTime, string location);
    }
}
