using UseCases.Clients.Commands;

namespace UseCases.Booking.Commands
{
    public class CancelBookingCommand : ICommand
    {
        public Guid BookingId { get; set; }
    }
}