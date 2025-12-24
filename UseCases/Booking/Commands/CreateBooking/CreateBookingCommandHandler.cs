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
using UseCases.Interfaces.Repositories;

namespace UseCases.Booking.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, Guid>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IPropertyRepository _propertyRepository;

        public CreateBookingCommandHandler(
            IBookingRepository bookingRepository,
            IClientRepository clientRepository,
            IPropertyRepository propertyRepository)
        {
            _bookingRepository = bookingRepository;
            _clientRepository = clientRepository;
            _propertyRepository = propertyRepository;
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

            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Guid>(
                    $"Client with ID {command.ClientId} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _propertyRepository.GetByIdAsync(propertyIdResult.Value);
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
            var existingBookingsResult = await _bookingRepository.GetByPropertyIdAsync(propertyIdResult.Value);
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
            var saveResult = await _bookingRepository.SaveAsync(bookingResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Guid>(saveResult.Error);
            }

            return Result.Success(bookingResult.Value.Id.Value);
        }
    }
}