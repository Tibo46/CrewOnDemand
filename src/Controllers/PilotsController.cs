using System.Threading.Tasks;
using CrewOnDemand.Models;
using CrewOnDemand.Models.Enums;
using CrewOnDemand.Models.Requests;
using CrewOnDemand.Repositories;
using CrewOnDemand.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrewOnDemand.Controllers
{
    [Route("[controller]")]
    public class PilotsController : Controller
    {
        private readonly ICrewOnDemandService _crewOnDemandService;

        public PilotsController(ICrewOnDemandService crewOnDemandService)
        {
            _crewOnDemandService = crewOnDemandService;
        }

        [HttpGet("availability")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pilot))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Pilot>> Availability([FromQuery] AvailabilitySearch search)
        {
            var (availabilityStatus, availablePilot) = await _crewOnDemandService.GetAvailablePilot(search.DepDateTime, search.ReturnDateTime, search.Location);

            switch (availabilityStatus)
            {
                case AvailabilityResponseStatus.WrongDates:
                    return BadRequest("Return date must be after Departure date");
                default:
                    return Ok(availablePilot);
            }
        }
    }
}
