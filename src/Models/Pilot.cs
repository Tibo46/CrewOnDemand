using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CrewOnDemand.Models
{
    [Table("Pilots")]
    public class Pilot
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [JsonIgnore]
        [Required]
        public int BaseId { get; set; }
        [JsonIgnore]
        public Base Base { get; set; }
        [JsonIgnore]
        [Required]
        public ICollection<WorkDay> WorkDays { get; set; }
        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; }

        [JsonIgnore]
        public DateTime CreationDate { get; set; }
    }
}
