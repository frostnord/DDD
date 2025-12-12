using Domain.Booking;
using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.CreateBooking
{
    public class CreateBookingCommand : ICommand<BookingEntity>
    {
        public Guid ClientId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid AgencyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}