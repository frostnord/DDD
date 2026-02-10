using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Booking.Commands.ConfirmBooking;

public class ConfirmBookingCommandHandler : ICommandHandler<ConfirmBookingCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ConfirmBookingCommand command)
    {
        var bookingId = BookingId.Create(command.BookingId);
        if (bookingId.IsFailure)
        {
            return Result.Failure($"Invalid booking ID: {bookingId.Error}");
        }

        var bookingResult = await _unitOfWork.Bookings.GetByIdAsync(bookingId.Value);
        if (bookingResult.IsFailure)
        {
            return Result.Failure($"Booking with ID {command.BookingId} not found");
        }

        var booking = bookingResult.Value;
        booking.Confirm();

        var saveResult = _unitOfWork.Bookings.Save(booking);
        if (saveResult.IsFailure)
        {
            return Result.Failure(saveResult.Error);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
