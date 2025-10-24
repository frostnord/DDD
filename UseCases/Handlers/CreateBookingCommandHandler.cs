
using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Booking;
using Domain.Domain.Booking.Booking;
using Domain.Domain.Booking.VO;
using Domain.Domain.Property.VO;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, Booking>
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

        public async Task<Result<Booking>> HandleAsync(CreateBookingCommand command)
        {
            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(command.ClientId);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Booking>($"Client with ID {command.ClientId.Value} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _propertyRepository.GetByIdAsync(command.PropertyId);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<Booking>($"Property with ID {command.PropertyId.Value} does not exist");
            }

            // Проверяем, что недвижимость доступна для бронирования
            var property = propertyResult.Value;
            if (property.Status != PropertyStatus.ForSale)
            {
                return Result.Failure<Booking>($"Property with ID {command.PropertyId.Value} is not available for booking");
            }

            // Проверяем, что на эту дату нет уже забронированных визитов
            var existingBookingsResult = await _bookingRepository.GetByPropertyIdAsync(command.PropertyId);
            if (existingBookingsResult.IsFailure)
            {
                return Result.Failure<Booking>($"Failed to retrieve existing bookings for property with ID {command.PropertyId.Value}");
            }
            
            var existingBookings = existingBookingsResult.Value;
            var hasConflictingBooking = existingBookings.Any(b =>
                b.BookingPeriod.StartDate.Date == command.VisitDate.Date ||
                b.BookingPeriod.EndDate.Date == command.VisitDate.Date ||
                (command.VisitDate.Date >= b.BookingPeriod.StartDate.Date &&
                 command.VisitDate.Date <= b.BookingPeriod.EndDate.Date));

            if (hasConflictingBooking)
            {
                return Result.Failure<Booking>($"Property with ID {command.PropertyId.Value} is already booked for the requested date");
            }

            // Создаем период бронирования (один день визита)
            var bookingPeriod = Period.Create(command.VisitDate, command.VisitDate).Value;

            // Создаем бронирование
            var bookingResult = Booking.Create(
                clientResult.Value,
                propertyResult.Value,
                null, // Агентство может быть добавлено позже
                bookingPeriod,
                property.Price
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