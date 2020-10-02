using CrewOnDemand.Models;
using CrewOnDemand.Models.Enums;
using System;
using System.Threading.Tasks;

namespace CrewOnDemand.Services
{
    public interface ICrewOnDemandService
    {
        Task<BookingResponseStatus> BookPilot(DateTime departureDateTime, DateTime returnDateTime, int pilotId);
        Task<(AvailabilityResponseStatus, Pilot)> GetAvailablePilot(DateTime departureDateTime, DateTime returnDateTime, string location);
    }
}
