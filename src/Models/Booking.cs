using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrewOnDemand.Models
{
    [Table("Bookings")]
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PilotId { get; set; }

        [Required]
        public DateTime DepartureDateTime { get; set; }
        [Required]
        public DateTime ReturnDateTime { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
