using CrewOnDemand.Events;
using CrewOnDemand.Models;
using CrewOnDemand.Models.Enums;
using CrewOnDemand.Repositories;
using CrewOnDemand.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CrewOnDemand_UnitTests.Services
{
    public class CrewOnDemandServiceTests
    {
        private CrewOnDemandService _sut;
        private Mock<ICrewOnDemandRepository> _repositoryMock;
        private Mock<IBookingCreatedPublisher> _publisherMock;
        private readonly TelemetryClient _telemetryClient;

        public CrewOnDemandServiceTests()
        {
            _repositoryMock = new Mock<ICrewOnDemandRepository>();
            _publisherMock = new Mock<IBookingCreatedPublisher>();
            _telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());
            _sut = new CrewOnDemandService(_repositoryMock.Object, _telemetryClient, _publisherMock.Object);
        }

        [Fact]
        public async void BookPilot_Returns_WrongDates_When_DepartureAfterReturn()
        {
            var result = await _sut.BookPilot(new DateTime(2020, 2, 10), new DateTime(1984, 2, 10), 10);

            Assert.Equal(BookingResponseStatus.WrongDates, result);
        }

        [Fact]
        public async void BookPilot_Returns_PilotNotAvailable_When_NoAvailability()
        {
            var depDateTime = new DateTime(1984, 08, 09);
            var returnDateTime = new DateTime(2020, 01, 02);
            var pilotId = 1;

            var expectedResult = new Tuple<int, Booking>(0, null);
            _repositoryMock.Setup(x => x.BookAsync(depDateTime, returnDateTime, pilotId))
                .ReturnsAsync(await Task.FromResult(expectedResult));

            var result = await _sut.BookPilot(depDateTime, returnDateTime, pilotId);

            Assert.Equal(BookingResponseStatus.PilotNotAvailable, result);
        }

        [Fact]
        public async void BookPilot_Returns_Error_When_BookingFailed()
        {
            var depDateTime = new DateTime(1984, 08, 09);
            var returnDateTime = new DateTime(2020, 01, 02);
            var pilotId = 1;

            var expectedResult = new Tuple<int, Booking>(0, new Booking());
            _repositoryMock.Setup(x => x.BookAsync(depDateTime, returnDateTime, pilotId))
                .ReturnsAsync(await Task.FromResult(expectedResult));

            var result = await _sut.BookPilot(depDateTime, returnDateTime, pilotId);

            Assert.Equal(BookingResponseStatus.Error, result);
        }

        [Fact]
        public async void BookPilot_Returns_Success_When_BookingSucceeded()
        {
            var depDateTime = new DateTime(1984, 08, 09);
            var returnDateTime = new DateTime(2020, 01, 02);
            var pilotId = 1;

            var expectedResult = new Tuple<int, Booking>(1, new Booking());
            _repositoryMock.Setup(x => x.BookAsync(depDateTime, returnDateTime, pilotId))
                .ReturnsAsync(await Task.FromResult(expectedResult));

            var result = await _sut.BookPilot(depDateTime, returnDateTime, pilotId);

            Assert.Equal(BookingResponseStatus.Success, result);
            Assert.Equal(1, _publisherMock.Invocations.Count);
        }
    }
}
