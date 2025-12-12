using UseCases.Clients.Commands;

namespace UseCases.Booking.Commands
{
    public class ConfirmBookingCommand : ICommand
    {
        public Guid BookingId { get; set; }
    }
}