using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CrewOnDemand.Models
{
    [Table("Bases")]
    public class Base
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [JsonIgnore]
        public DateTime CreationDate { get; set; }
    }
}
