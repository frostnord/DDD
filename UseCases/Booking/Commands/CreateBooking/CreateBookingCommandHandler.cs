using System;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Booking.Commands.CreateBooking;

public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateBookingCommand command)
    {
        // Создаем Value Objects из примитивных значений
        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid client ID: {clientIdResult.Error}");
        }

        var propertyIdResult = PropertyId.Create(command.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid property ID: {propertyIdResult.Error}");
        }

        var bookingPeriodResult = Period.Create(command.StartDate, command.EndDate);
        if (bookingPeriodResult.IsFailure)
        {
            return Result.Failure<Guid>(
                $"Invalid booking period: {bookingPeriodResult.Error}");
        }

        var totalPriceResult = Price.Create(command.TotalPrice);
        if (totalPriceResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid total price: {totalPriceResult.Error}");
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async _ =>
        {
            // Проверяем, существует ли клиент
            var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Guid>(
                    $"Client with ID {command.ClientId} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _unitOfWork.Properties.GetByIdForUpdateAsync(propertyIdResult.Value);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<Guid>(
                    $"Property with ID {command.PropertyId} does not exist");
            }

            // Проверяем, что недвижимость доступна для бронирования
            var property = propertyResult.Value;
            if (property.Status != PropertyStatus.ForSale)
            {
                return Result.Failure<Guid>(
                    $"Property with ID {command.PropertyId} is not available for booking");
            }

            // Проверяем, что на этот период нет уже забронированных визитов
            var existingBookingsResult = await _unitOfWork.Bookings.GetByPropertyIdAsync(propertyIdResult.Value);
            if (existingBookingsResult.IsFailure)
            {
                return Result.Failure<Guid>(
                    $"Failed to retrieve existing bookings for property with ID {command.PropertyId}");
            }

            var existingBookings = existingBookingsResult.Value;
            var hasConflictingBooking = existingBookings.Any(b =>
                (bookingPeriodResult.Value.StartDate.Date <= b.BookingPeriod.EndDate.Date &&
                 bookingPeriodResult.Value.EndDate.Date >= b.BookingPeriod.StartDate.Date));

            if (hasConflictingBooking)
            {
                return Result.Failure<Guid>(
                    $"Property with ID {command.PropertyId} is already booked for the requested period");
            }

            // Создаем бронирование
            var bookingResult = BookingEntity.Create(
                clientIdResult.Value,
                propertyIdResult.Value,
                bookingPeriodResult.Value,
                totalPriceResult.Value
            );

            if (bookingResult.IsFailure)
            {
                return Result.Failure<Guid>(bookingResult.Error);
            }

            // Сохраняем бронирование
            var saveResult = _unitOfWork.Bookings.Save(bookingResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Guid>(saveResult.Error);
            }

            var reserveResult = property.Reserve();
            if (reserveResult.IsFailure)
            {
                return Result.Failure<Guid>(reserveResult.Error);
            }

            return Result.Success(bookingResult.Value.Id.Value);
        });
    }
}
