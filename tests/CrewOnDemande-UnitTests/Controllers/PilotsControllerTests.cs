using CrewOnDemand.Controllers;
using CrewOnDemand.Models;
using CrewOnDemand.Models.Enums;
using CrewOnDemand.Models.Requests;
using CrewOnDemand.Repositories;
using CrewOnDemand.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CrewOnDemand_UnitTests.Controllers
{
    public class PilotsControllerTests
    {
        private PilotsController _sut;
        private Mock<ICrewOnDemandService> _serviceMock;

        public PilotsControllerTests()
        {
            _serviceMock = new Mock<ICrewOnDemandService>();
            _sut = new PilotsController(_serviceMock.Object);
        }

        [Fact]
        public async void Availability_Returns_BadRequest_When_DepartureAfterReturn()
        {
            var depDateTime = new DateTime(2020, 08, 09);
            var returnDateTime = new DateTime(1984, 01, 02);
            var location = "munich";

            _serviceMock.Setup(x => x.GetAvailablePilot(depDateTime, returnDateTime, location))
                .ReturnsAsync(await Task.FromResult((AvailabilityResponseStatus.WrongDates, new Pilot())));

            var result = await _sut.Availability(new AvailabilitySearch
            {
                DepDateTime = depDateTime,
                ReturnDateTime = returnDateTime,
                Location = location
            });
            var status = result.Result as BadRequestObjectResult;

            Assert.Equal(400, status.StatusCode);
        }


        [Fact]
        public async void Availability_Returns_BadRequest_When_ParametersMissing()
        {
            var result = await _sut.Availability(null);

            var status = result.Result as BadRequestObjectResult;

            Assert.Equal(400, status.StatusCode);
        }

        [Fact]
        public async void Availability_Returns_Pilot_When_ParametersAreCorrect()
        {
            var depDateTime = new DateTime(1984, 08, 09);
            var returnDateTime = new DateTime(2020, 01, 02);
            var location = "munich";

            _serviceMock.Setup(x => x.GetAvailablePilot(depDateTime, returnDateTime, location))
                .ReturnsAsync(await Task.FromResult((AvailabilityResponseStatus.Success, new Pilot
                {
                    Id = 1,
                    Name = "Walter White"
                })));

            var result = await _sut.Availability(new AvailabilitySearch
            {
                DepDateTime = depDateTime,
                ReturnDateTime = returnDateTime,
                Location = location
            });

            var status = result.Result as OkObjectResult;

            Assert.Equal(200, status.StatusCode);
        }
    }
}
