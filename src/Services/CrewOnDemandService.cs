using CrewOnDemand.Events;
using CrewOnDemand.Models;
using CrewOnDemand.Models.Enums;
using CrewOnDemand.Repositories;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Threading.Tasks;

namespace CrewOnDemand.Services
{
    public class CrewOnDemandService: ICrewOnDemandService
    {
        private readonly ICrewOnDemandRepository _crewOnDemandRepository;
        private readonly TelemetryClient _telemetryClient;
        private readonly IBookingCreatedPublisher _bookingCreatedPublisher;

        public CrewOnDemandService(ICrewOnDemandRepository crewOnDemandRepository, TelemetryClient telemetryClient, IBookingCreatedPublisher bookingCreatedPublisher)
        {
            _crewOnDemandRepository = crewOnDemandRepository;
            _telemetryClient = telemetryClient;
            _bookingCreatedPublisher = bookingCreatedPublisher;
        }

        public async Task<BookingResponseStatus> BookPilot(DateTime departureDateTime, DateTime returnDateTime, int pilotId)
        {
            if (departureDateTime >= returnDateTime)
            {
                return BookingResponseStatus.WrongDates;
            }

            var (numberOfBookingSaved, newBooking) = await _crewOnDemandRepository.BookAsync(departureDateTime, returnDateTime, pilotId);

            if(newBooking == null)
            {
                return BookingResponseStatus.PilotNotAvailable;
            }

            if (numberOfBookingSaved == 0)
            {
                var eventTelemetry = new EventTelemetry("Pilot available but booking not saved");
                eventTelemetry.Properties.Add("PilotId", pilotId.ToString());
                eventTelemetry.Properties.Add("DepartureDateTime", departureDateTime.ToString());
                eventTelemetry.Properties.Add("ReturnDateTime", returnDateTime.ToString());
                _telemetryClient.TrackEvent(eventTelemetry);

                return BookingResponseStatus.Error;
            }

            await _bookingCreatedPublisher.SendAsync(newBooking);

            return BookingResponseStatus.Success;
        }

        public async Task<(AvailabilityResponseStatus, Pilot)> GetAvailablePilot(DateTime departureDateTime, DateTime returnDateTime, string location)
        {
            if (departureDateTime >= returnDateTime)
            {
                return (AvailabilityResponseStatus.WrongDates, null);
            }

            var availablePilot = await _crewOnDemandRepository.GetAvailablePilotAsync(departureDateTime, returnDateTime, location);

            if (availablePilot == null)
            {
                var eventTelemetry = new EventTelemetry("No pilot available for this search, let's hire more pilots!");
                eventTelemetry.Properties.Add("Location", location);
                eventTelemetry.Properties.Add("DepartureDateTime", departureDateTime.ToString());
                eventTelemetry.Properties.Add("ReturnDateTime", returnDateTime.ToString());
                _telemetryClient.TrackEvent(eventTelemetry);
            }

            return (AvailabilityResponseStatus.Success, availablePilot);
        }
    }
}
