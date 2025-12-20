using Domain.Booking;
using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.CreateBooking
{
    public sealed record CreateBookingCommand(
        Guid ClientId,
        Guid PropertyId,
        Guid AgencyId,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalPrice
    ) : ICommand<BookingEntity>;
}