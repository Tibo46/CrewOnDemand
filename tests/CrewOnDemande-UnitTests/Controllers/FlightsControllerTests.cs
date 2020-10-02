using CrewOnDemand.Controllers;
using CrewOnDemand.Models.Enums;
using CrewOnDemand.Models.Requests;
using CrewOnDemand.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CrewOnDemand_UnitTests.Controllers
{
    public class FlightsControllerTests
    {
        private FlightsController _sut;
        private Mock<ICrewOnDemandService> _serviceMock;

        public FlightsControllerTests()
        {
            _serviceMock = new Mock<ICrewOnDemandService>();
            _sut = new FlightsController(_serviceMock.Object);
        }

        [Fact]
        public async void Book_Returns_BadRequest_When_DepartureAfterReturn()
        {
            var depDateTime = new DateTime(2020, 08, 09);
            var returnDateTime = new DateTime(1984, 01, 02);
            var pilotId = 1;

            _serviceMock.Setup(x => x.BookPilot(depDateTime, returnDateTime, pilotId))
                .ReturnsAsync(await Task.FromResult(BookingResponseStatus.WrongDates));

            var result = await _sut.Book(new BookingRequest
            {
                DepDateTime = depDateTime,
                ReturnDateTime = returnDateTime,
                PilotId = pilotId
            });

            var status = result as BadRequestObjectResult;

            Assert.Equal(400, status.StatusCode);
        }

        [Fact]
        public async void Book_Returns_Created_When_BookingIsComplete()
        {
            var depDateTime = new DateTime(1984, 08, 09);
            var returnDateTime = new DateTime(2020, 01, 02);
            var pilotId = 1;

            _serviceMock.Setup(x => x.BookPilot(depDateTime, returnDateTime, pilotId))
                .ReturnsAsync(await Task.FromResult(BookingResponseStatus.Success));

            var result = await _sut.Book(new BookingRequest
            {
                DepDateTime = depDateTime,
                ReturnDateTime = returnDateTime,
                PilotId = pilotId
            });

            var status = result as StatusCodeResult;

            Assert.Equal(201, status.StatusCode);
        }

        [Fact]
        public async void Book_Returns_Conflict_When_PilotNotAvailable()
        {
            var depDateTime = new DateTime(1984, 08, 09);
            var returnDateTime = new DateTime(2020, 01, 02);
            var pilotId = 1;

            _serviceMock.Setup(x => x.BookPilot(depDateTime, returnDateTime, pilotId))
                .ReturnsAsync(await Task.FromResult(BookingResponseStatus.PilotNotAvailable));

            var result = await _sut.Book(new BookingRequest
            {
                DepDateTime = depDateTime,
                ReturnDateTime = returnDateTime,
                PilotId = pilotId
            });

            var status = result as ConflictObjectResult;

            Assert.Equal(409, status.StatusCode);
        }
    }
}
