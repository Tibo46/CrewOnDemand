using CrewOnDemand.Controllers;
using CrewOnDemand.Database;
using CrewOnDemand.Events;
using CrewOnDemand.Models;
using CrewOnDemand.Models.Requests;
using CrewOnDemand.Repositories;
using CrewOnDemand.Services;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace CrewOnDemand_IntegrationTests.Controllers
{
    public class PilotsControllerTests
    {
        private readonly List<Booking> _fakeBookings;
        private readonly TelemetryClient _telemetryClient;

        public static readonly object[][] CorrectData =
        {
            new object[] { new DateTime(2020, 9, 28, 4, 0, 0), new DateTime(2020, 9, 28, 5, 30, 0), "munich", 1 },
            new object[] { new DateTime(2020, 10, 2, 6, 0, 0), new DateTime(2020, 10, 2, 10, 0, 0), "Munich", 4 },
            new object[] { new DateTime(2020, 10, 1, 6, 0, 0), new DateTime(2020, 10, 3, 10, 0, 0), "berlin", 5 },
        };

        public static readonly object[][] NoResultData =
        {
            new object[] { new DateTime(2020, 9, 28, 6, 0, 0), new DateTime(2020, 9, 28, 10, 0, 0), "munich" },
            new object[] { new DateTime(2020, 9, 30, 6, 0, 0), new DateTime(2020, 10, 1, 10, 0, 0), "Munich" },
            new object[] { new DateTime(2020, 10, 1, 6, 0, 0), new DateTime(2020, 10, 3, 10, 0, 0), "iAmNotACity" },
        };


        public PilotsControllerTests()
        {
            _fakeBookings = GenerateMockBookings();
            _telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());
        }

        [Fact]
        public async void Availability_Returns_BadRequest_When_DepartureAfterReturn()
        {
            var options = new DbContextOptionsBuilder<CrewOnDemandContext>()
                .UseInMemoryDatabase(databaseName: "db_empty")
                .Options;

            using (var context = new CrewOnDemandContext(options))
            {

                var repository = new CrewOnDemandRepository(context);
                var service = new CrewOnDemandService(repository, _telemetryClient, new BookingCreatedPublisher(_telemetryClient));
                var sut = new PilotsController(service);

                var actionResult = await sut.Availability(new AvailabilitySearch
                {
                    DepDateTime = new DateTime(2020, 1, 1),
                    ReturnDateTime = new DateTime(2000, 1, 1),
                    Location = "munich"
                });
                Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            }
        }

        [Theory, MemberData(nameof(CorrectData))]
        public async void Availability_Returns_CorrectResult(DateTime depDate, DateTime returnDate, string location, int expectedPilotId)
        {
            var options = new DbContextOptionsBuilder<CrewOnDemandContext>()
                .UseInMemoryDatabase(databaseName: "db_seeded_with_bookings_1")
                .Options;

            using (var context = new CrewOnDemandContext(options))
            {
                context.Database.EnsureCreated();
                context.Bookings.AddRange(_fakeBookings);
                context.SaveChanges();

                var repository = new CrewOnDemandRepository(context);
                var service = new CrewOnDemandService(repository, _telemetryClient, new BookingCreatedPublisher(_telemetryClient));
                var sut = new PilotsController(service);

                var actionResult = await sut.Availability(new AvailabilitySearch
                {
                    DepDateTime = depDate,
                    ReturnDateTime = returnDate,
                    Location = location
                });
                Assert.IsType<OkObjectResult>(actionResult.Result);

                var result = actionResult.Result as OkObjectResult;
                var pilotResult = result.Value as Pilot;

                Assert.Equal(expectedPilotId, pilotResult.Id);
            }
        }

        [Theory, MemberData(nameof(NoResultData))]
        public async void Availability_Returns_NoResult(DateTime depDate, DateTime returnDate, string location)
        {
            var options = new DbContextOptionsBuilder<CrewOnDemandContext>()
                .UseInMemoryDatabase(databaseName: "db_seeded_with_bookings_2")
                .Options;

            using (var context = new CrewOnDemandContext(options))
            {
                context.Database.EnsureCreated();
                context.Bookings.AddRange(_fakeBookings);
                context.SaveChanges();

                var repository = new CrewOnDemandRepository(context);
                var service = new CrewOnDemandService(repository, _telemetryClient, new BookingCreatedPublisher(_telemetryClient));
                var sut = new PilotsController(service);

                var actionResult = await sut.Availability(new AvailabilitySearch
                {
                    DepDateTime = depDate,
                    ReturnDateTime = returnDate,
                    Location = location
                });
                Assert.IsType<OkObjectResult>(actionResult.Result);

                var result = actionResult.Result as OkObjectResult;
                var pilotResult = result.Value as Pilot;

                Assert.Null(pilotResult);
            }
        }

        private List<Booking> GenerateMockBookings()
        {
            return new List<Booking>
            {
                new Booking
                {
                    PilotId = 1,
                    DepartureDateTime = new DateTime(2020, 9, 28, 8, 20, 0),
                    ReturnDateTime = new DateTime(2020, 9, 28, 15, 30, 0)
                },
                new Booking
                {
                    PilotId = 2,
                    DepartureDateTime = new DateTime(2020, 9, 26, 8, 20, 0),
                    ReturnDateTime = new DateTime(2020, 9, 28, 15, 30, 0)
                },
                new Booking
                {
                    PilotId = 3,
                    DepartureDateTime = new DateTime(2020, 9, 30, 6, 0, 0),
                    ReturnDateTime = new DateTime(2020, 10, 1, 10, 0, 0)
                }
            };
        }
    }
}