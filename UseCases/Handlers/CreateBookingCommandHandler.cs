

using CSharpFunctionalExtensions;
using Domain.Domain.Agency.VO;
using Domain.Domain.Booking.VO;
using Domain.Domain.Booking;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;
using UseCases.Booking.Commands;
using UseCases.Clients.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, Domain.Domain.Booking.Booking>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IAgencyRepository _agencyRepository;

        public CreateBookingCommandHandler(
            IBookingRepository bookingRepository,
            IClientRepository clientRepository,
            IPropertyRepository propertyRepository,
            IAgencyRepository agencyRepository)
        {
            _bookingRepository = bookingRepository;
            _clientRepository = clientRepository;
            _propertyRepository = propertyRepository;
            _agencyRepository = agencyRepository;
        }

        public async Task<Result<Domain.Domain.Booking.Booking>> HandleAsync(CreateBookingCommand command)
        {
            // Создаем Value Objects из примитивных значений
            var clientIdResult = ClientId.Create(command.ClientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<Domain.Domain.Booking.Booking>($"Invalid client ID: {clientIdResult.Error}");
            }

            var propertyIdResult = PropertyId.Create(command.PropertyId);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure<Domain.Domain.Booking.Booking>($"Invalid property ID: {propertyIdResult.Error}");
            }

            var agencyIdResult = AgencyId.Create(command.AgencyId);
            if (agencyIdResult.IsFailure)
            {
                return Result.Failure<Booking>($"Invalid agency ID: {agencyIdResult.Error}");
            }

            var bookingPeriodResult = Period.Create(command.StartDate, command.EndDate);
            if (bookingPeriodResult.IsFailure)
            {
                return Result.Failure<Domain.Domain.Booking.Booking>($"Invalid booking period: {bookingPeriodResult.Error}");
            }

            var totalPriceResult = Price.Create(command.TotalPrice);
            if (totalPriceResult.IsFailure)
            {
                return Result.Failure<Domain.Domain.Booking.Booking>($"Invalid total price: {totalPriceResult.Error}");
            }

            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Booking>($"Client with ID {command.ClientId} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _propertyRepository.GetByIdAsync(propertyIdResult.Value);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<Booking>($"Property with ID {command.PropertyId} does not exist");
            }

            // Проверяем, существует ли агентство
            var agencyResult = await _agencyRepository.GetByIdAsync(agencyIdResult.Value);
            if (agencyResult.IsFailure)
            {
                return Result.Failure<Booking>($"Agency with ID {command.AgencyId} does not exist");
            }

            // Проверяем, что недвижимость доступна для бронирования
            var property = propertyResult.Value;
            if (property.Status != PropertyStatus.ForSale)
            {
                return Result.Failure<Booking>(
                    $"Property with ID {command.PropertyId} is not available for booking");
            }

            // Проверяем, что на этот период нет уже забронированных визитов
            var existingBookingsResult = await _bookingRepository.GetByPropertyIdAsync(propertyIdResult.Value);
            if (existingBookingsResult.IsFailure)
            {
                return Result.Failure<Booking>(
                    $"Failed to retrieve existing bookings for property with ID {command.PropertyId}");
            }

            var existingBookings = existingBookingsResult.Value;
            var hasConflictingBooking = existingBookings.Any(b =>
                (bookingPeriodResult.Value.StartDate.Date <= b.BookingPeriod.EndDate.Date &&
                 bookingPeriodResult.Value.EndDate.Date >= b.BookingPeriod.StartDate.Date));

            if (hasConflictingBooking)
            {
                return Result.Failure<Booking>(
                    $"Property with ID {command.PropertyId} is already booked for the requested period");
            }

            // Создаем бронирование
            var bookingResult = Booking.Create(
                clientIdResult.Value,
                propertyIdResult.Value,
                agencyIdResult.Value,
                bookingPeriodResult.Value,
                totalPriceResult.Value
            );

            if (bookingResult.IsFailure)
            {
                return Result.Failure<Booking>(bookingResult.Error);
            }

            // Сохраняем бронирование
            var saveResult = await _bookingRepository.SaveAsync(bookingResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Booking>(saveResult.Error);
            }

            return Result.Success(bookingResult.Value);
        }
    }
}