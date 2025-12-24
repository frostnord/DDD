using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Deal.Commands
{
    public class CreateDealCommandHandler : ICommandHandler<CreateDealCommand, DealEntity>
    {
        private readonly IDealRepository _dealRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IClientRepository _clientRepository;

        public CreateDealCommandHandler(
            IDealRepository dealRepository,
            IPropertyRepository propertyRepository,
            IClientRepository clientRepository)
        {
            _dealRepository = dealRepository;
            _propertyRepository = propertyRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result<DealEntity>> HandleAsync(CreateDealCommand command)
        {
            // Создаем ValueObject из простых типов данных
            var clientIdResult = ClientId.Create(command.ClientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Invalid client ID: {command.ClientId}");
            }

            var propertyIdResult = PropertyId.Create(command.PropertyId);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Invalid property ID: {command.PropertyId}");
            }

            BookingId? bookingId = null;
            if (command.BookingId.HasValue)
            {
                var bookingIdResult = BookingId.Create(command.BookingId.Value);
                if (bookingIdResult.IsFailure)
                {
                    return Result.Failure<DealEntity>($"Invalid booking ID: {command.BookingId.Value}");
                }
                bookingId = bookingIdResult.Value;
            }

            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Client with ID {command.ClientId} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _propertyRepository.GetByIdAsync(propertyIdResult.Value);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Property with ID {command.PropertyId} does not exist");
            }

            // Проверяем статус недвижимости
            var property = propertyResult.Value;
            if (property.Status != PropertyStatus.ForSale)
            {
                return Result.Failure<DealEntity>($"Property with ID {command.PropertyId} is not available for sale");
            }

            // Создаем сделку
            var dealResult = DealEntity.Create(
                clientIdResult.Value,
                propertyIdResult.Value,
                bookingId,
                command.Details
            );

            if (dealResult.IsFailure)
            {
                return Result.Failure<DealEntity>(dealResult.Error);
            }

            // Сохраняем сделку
            var saveResult = await _dealRepository.AddAsync(dealResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<DealEntity>(saveResult.Error);
            }

            return Result.Success(dealResult.Value);
        }
    }
}