using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.CancelBooking
{
    public class CancelBookingCommand : ICommand
    {
        public Guid BookingId { get; set; }
    }
}