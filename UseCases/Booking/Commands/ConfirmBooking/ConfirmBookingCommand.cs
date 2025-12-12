using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.ConfirmBooking
{
    public class ConfirmBookingCommand : ICommand
    {
        public Guid BookingId { get; set; }
    }
}