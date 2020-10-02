using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrewOnDemand.Models
{
    [Table("WorkDays")]
    public class WorkDay
    {
        [Key]
        public int PilotId { get; set; }
        public Pilot Pilot { get; set; }

        [Key]
        public int DayOfTheWeek { get; set; }
    }
}
