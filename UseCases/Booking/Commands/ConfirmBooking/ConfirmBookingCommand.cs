using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.ConfirmBooking
{
    public record ConfirmBookingCommand(Guid BookingId) : ICommand;
}