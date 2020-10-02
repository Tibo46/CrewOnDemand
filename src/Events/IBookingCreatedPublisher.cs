using CrewOnDemand.Models;
using System.Threading.Tasks;

namespace CrewOnDemand.Events
{
    public interface IBookingCreatedPublisher
    {
        Task SendAsync(Booking newBooking);
    }
}
