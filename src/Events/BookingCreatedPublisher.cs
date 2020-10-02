using CrewOnDemand.Models;
using CrewOnDemand.Models.Events;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Threading.Tasks;

namespace CrewOnDemand.Events
{
    public class BookingCreatedPublisher: IBookingCreatedPublisher
    {
        private readonly TelemetryClient _telemetryClient;

        public BookingCreatedPublisher(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public async Task SendAsync(Booking newBooking)
        {
            var eventObject = new Envelope<Booking>
            {
                Id = new Guid(),
                EventDate = DateTime.Now,
                Data = newBooking,
            };

            // Here the eventObject should be published.
            // To keep the project simple, and due to time constraint, I omitted this part

            await Task.CompletedTask;
            TrackBookingCreatedPublished(eventObject);
        }

        private void TrackBookingCreatedPublished(Envelope<Booking> eventObject)
        {
            var eventTelemetry = new EventTelemetry("Booking completed event published");
            eventTelemetry.Properties.Add("EventId", eventObject.Id.ToString());
            eventTelemetry.Properties.Add("BookingId", eventObject.Data.Id.ToString());
            _telemetryClient.TrackEvent(eventTelemetry);
        }
    }
}
