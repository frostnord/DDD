using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using UseCases.Booking.Commands;
using UseCases.Clients.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class ConfirmBookingCommandHandler : ICommandHandler<ConfirmBookingCommand>
    {
        private readonly IBookingRepository _bookingRepository;

        public ConfirmBookingCommandHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result> HandleAsync(ConfirmBookingCommand command)
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
            booking.Confirm();

            var saveResult = await _bookingRepository.SaveAsync(booking);
            if (saveResult.IsFailure)
            {
                return Result.Failure(saveResult.Error);
            }

            return Result.Success();
        }
    }
}