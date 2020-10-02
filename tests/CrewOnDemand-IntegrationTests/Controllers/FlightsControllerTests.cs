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
    public class FlightsControllerTests
    {
        private readonly List<Booking> _fakeBookings;
        private readonly TelemetryClient _telemetryClient;

        public static readonly object[][] CorrectData =
        {
            new object[] { new DateTime(2020, 9, 28, 4, 0, 0), new DateTime(2020, 9, 28, 5, 30, 0), 1, true },
            new object[] { new DateTime(2020, 10, 2, 6, 0, 0), new DateTime(2020, 10, 2, 10, 0, 0), 4, true },
            new object[] { new DateTime(2020, 10, 1, 6, 0, 0), new DateTime(2020, 10, 3, 10, 0, 0), 5, true },
            new object[] { new DateTime(2020, 9, 28, 6, 0, 0), new DateTime(2020, 9, 28, 10, 0, 0), 1, false },
            new object[] { new DateTime(2020, 9, 26, 6, 0, 0), new DateTime(2020, 10, 1, 10, 0, 0), 2, false },
            new object[] { new DateTime(2020, 10, 1, 6, 0, 0), new DateTime(2020, 10, 3, 10, 0, 0), 125404, false },
        };

        public FlightsControllerTests()
        {
            _fakeBookings = GenerateMockBookings();
            _telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());
        }

        [Fact]
        public async void Book_Returns_BadRequest_When_DepartureAfterReturn()
        {
            var options = new DbContextOptionsBuilder<CrewOnDemandContext>()
                .UseInMemoryDatabase(databaseName: "db_empty")
                .Options;

            using (var context = new CrewOnDemandContext(options))
            {
                var repository = new CrewOnDemandRepository(context);
                var service = new CrewOnDemandService(repository, _telemetryClient, new BookingCreatedPublisher(_telemetryClient));
                var sut = new FlightsController(service);

                var actionResult = await sut.Book(new BookingRequest
                {
                    DepDateTime = new DateTime(2020, 1, 1),
                    ReturnDateTime = new DateTime(2000, 1, 1),
                    PilotId = 1
                });
                Assert.IsType<BadRequestObjectResult>(actionResult);
            }
        }

        [Theory, MemberData(nameof(CorrectData))]
        public async void Book_Returns_CorrectResult(DateTime depDate, DateTime returnDate, int pilotId, bool expectedResult)
        {
            var options = new DbContextOptionsBuilder<CrewOnDemandContext>()
                .UseInMemoryDatabase(databaseName: "db_seeded_with_bookings")
                .Options;

            using (var context = new CrewOnDemandContext(options))
            {
                context.Database.EnsureCreated();
                context.Bookings.AddRange(_fakeBookings);
                context.SaveChanges();

                var repository = new CrewOnDemandRepository(context);
                var service = new CrewOnDemandService(repository, _telemetryClient, new BookingCreatedPublisher(_telemetryClient));
                var sut = new FlightsController(service);

                var actionResult = await sut.Book(new BookingRequest
                {
                    DepDateTime = depDate,
                    ReturnDateTime = returnDate,
                    PilotId = pilotId
                });

                if(expectedResult)
                {
                    Assert.IsType<StatusCodeResult>(actionResult);

                    var result = actionResult as StatusCodeResult;

                    Assert.Equal(201, result.StatusCode);
                }
                else
                {
                    Assert.IsType<ConflictObjectResult>(actionResult);
                }
                
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