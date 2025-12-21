using Domain.Booking;
using Domain.Booking.VO;
using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.CreateBooking
{
    public sealed record CreateBookingCommand(
        Guid ClientId,
        Guid PropertyId,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalPrice
    ) : ICommand<Guid>;
}