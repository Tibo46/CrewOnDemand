using System;
using System.ComponentModel.DataAnnotations;

namespace CrewOnDemand.Models.Requests
{
    public class DepartureReturnDates
    {
        [Required]
        public DateTime DepDateTime { get; set; }
        [Required]
        public DateTime ReturnDateTime { get; set; }
    }
}
