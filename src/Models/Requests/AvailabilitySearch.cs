using System.ComponentModel.DataAnnotations;

namespace CrewOnDemand.Models.Requests
{
    public class AvailabilitySearch: DepartureReturnDates
    {
        [Required]
        public string Location { get; set; }
    }
}
