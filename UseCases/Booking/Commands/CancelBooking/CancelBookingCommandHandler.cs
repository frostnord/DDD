using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Booking.Commands.CancelBooking;

public class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand>
{
    private readonly IBookingRepository _bookingRepository;

    public CancelBookingCommandHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<Result> HandleAsync(CancelBookingCommand command)
    {
        var bookingId = BookingId.Create(command.BookingId);
        if (bookingId.IsFailure)
        {
            return Result.Failure($"Invalid booking ID: {bookingId.Error}");
        }

        var bookingResult = await _bookingRepository.GetByIdAsync(bookingId.Value);
        if (bookingResult.IsFailure)
        {
            return Result.Failure($"Booking with ID {command.BookingId} not found");
        }

        var booking = bookingResult.Value;
        booking.Cancel();

        var saveResult = await _bookingRepository.SaveAsync(booking);
        if (saveResult.IsFailure)
        {
            return Result.Failure(saveResult.Error);
        }

        return Result.Success();
    }
}