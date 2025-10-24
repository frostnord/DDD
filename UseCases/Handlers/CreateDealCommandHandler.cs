using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Deal;
using Domain.Domain.Property.VO;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateDealCommandHandler : ICommandHandler<CreateDealCommand, Deal>
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

        public async Task<Result<Deal>> HandleAsync(CreateDealCommand command)
        {
            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(command.ClientId);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Deal>($"Client with ID {command.ClientId.Value} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _propertyRepository.GetByIdAsync(command.PropertyId);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<Deal>($"Property with ID {command.PropertyId.Value} does not exist");
            }

            // Проверяем статус недвижимости
            var property = propertyResult.Value;
            if (property.Status != PropertyStatus.ForSale)
            {
                return Result.Failure<Deal>($"Property with ID {command.PropertyId.Value} is not available for sale");
            }

            // Создаем сделку
            var dealResult = Deal.Create(
                command.ClientId,
                command.PropertyId,
                command.BookingId,
                command.Details
            );

            if (dealResult.IsFailure)
            {
                return Result.Failure<Deal>(dealResult.Error);
            }

            // Сохраняем сделку
            var saveResult = await _dealRepository.AddAsync(dealResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Deal>(saveResult.Error);
            }

            return Result.Success(dealResult.Value);
        }
    }
}