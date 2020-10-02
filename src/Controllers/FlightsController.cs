using System;
using System.Linq;
using System.Threading.Tasks;
using CrewOnDemand.Database;
using CrewOnDemand.Models;
using CrewOnDemand.Models.Enums;
using CrewOnDemand.Models.Requests;
using CrewOnDemand.Repositories;
using CrewOnDemand.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrewOnDemand.Controllers
{
    [Route("[controller]")]
    public class FlightsController : Controller
    {
        private readonly ICrewOnDemandService _crewOnDemandService;

        public FlightsController(ICrewOnDemandService crewOnDemandService)
        {
            _crewOnDemandService = crewOnDemandService;
        }


        [HttpPost("book")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Book([FromBody] BookingRequest bookingRequest)
        {
            if(bookingRequest == null) {
                return BadRequest("All parameters are required");
            }

            var bookingStatus = await _crewOnDemandService.BookPilot(bookingRequest.DepDateTime, bookingRequest.ReturnDateTime, bookingRequest.PilotId);

            switch (bookingStatus)
            {
                case BookingResponseStatus.WrongDates:
                    return BadRequest("Return date must be after Departure date");
                case BookingResponseStatus.PilotNotAvailable:
                    return Conflict("This pilot is not available on the given dates");
                case BookingResponseStatus.Error:
                    return Problem("An error occured and the booking failed");
                default:
                    return new StatusCodeResult(201);
            }
        }
    }
}
