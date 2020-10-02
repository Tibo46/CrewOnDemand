using System;
using System.ComponentModel.DataAnnotations;

namespace CrewOnDemand.Models.Requests
{
    public class BookingRequest : DepartureReturnDates
    {
        [Required]
        public int PilotId { get; set; }
    }
}
