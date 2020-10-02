using System;

namespace CrewOnDemand.Models.Events
{
    public class Envelope<T>
    {
        public Guid Id { get; set; }
        public DateTime EventDate { get; set; }
        public T Data { get; set; }
    }
}
