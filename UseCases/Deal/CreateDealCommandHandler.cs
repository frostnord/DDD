using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Deal
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
            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(command.ClientId);
            if (clientResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Client with ID {command.ClientId.Value} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _propertyRepository.GetByIdAsync(command.PropertyId);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<DealEntity>($"Property with ID {command.PropertyId.Value} does not exist");
            }

            // Проверяем статус недвижимости
            var property = propertyResult.Value;
            if (property.Status != PropertyStatus.ForSale)
            {
                return Result.Failure<DealEntity>($"Property with ID {command.PropertyId.Value} is not available for sale");
            }

            // Создаем сделку
            var dealResult = DealEntity.Create(
                command.ClientId,
                command.PropertyId,
                command.BookingId,
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