using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO; // Добавляем using для SmartPropertyType, HeatingType, PropertyCondition
using Domain.ValueObjects; // Добавляем using для NumberOfRooms, Floor, TotalFloors
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Buyer.Commands.UpdateBuyer
{
    public class UpdateBuyerCommandHandler : ICommandHandler<UpdateBuyerCommand>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IClientRepository _clientRepository;

        public UpdateBuyerCommandHandler(
            IBuyerRepository buyerRepository,
            IClientRepository clientRepository)
        {
            _buyerRepository = buyerRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result> HandleAsync(UpdateBuyerCommand command)
        {
            var clientIdResult = ClientId.Create(command.ClientId);
            if (clientIdResult.IsFailure)
                return Result.Failure(clientIdResult.Error);

            var clientExists = await _clientRepository.ExistsAsync(clientIdResult.Value);
            if (!clientExists)
            {
                return Result.Failure($"Client with ID {command.ClientId} does not exist");
            }

            var buyerIdResult = BuyerId.Create(command.BuyerId);
            if (buyerIdResult.IsFailure)
                return Result.Failure(buyerIdResult.Error);

            var buyerResult = await _buyerRepository.GetByIdAsync(buyerIdResult.Value);
            if (buyerResult.IsFailure)
            {
                return Result.Failure($"Buyer with ID {command.BuyerId} does not exist");
            }

            // Создаем Value Objects для ClientSearchCriteria
            var numberOfRoomsResult = NumberOfRooms.Create(command.PreferredNumberOfRooms);
            var floorResult = Floor.Create(command.PreferredFloor);
            var totalFloorsResult = TotalFloors.Create(command.PreferredTotalFloors);
            var propertyTypeResult = SmartPropertyType.FromName(command.PreferredType);
            var heatingTypeResult = HeatingType.Create(command.PreferredHeatingType);
            var conditionResult = PropertyCondition.Create(command.PreferredCondition);

            var combinedResult = Result.Combine(
                numberOfRoomsResult,
                floorResult,
                totalFloorsResult,
                heatingTypeResult,
                conditionResult
            );

            if (combinedResult.IsFailure)
            {
                return Result.Failure(combinedResult.Error);
            }
            if (propertyTypeResult == null)
            {
                return Result.Failure("Invalid property type");
            }

            var searchCriteriaResult = ClientSearchCriteria.Create(
                numberOfRoomsResult.Value,
                floorResult.Value,
                totalFloorsResult.Value,
                propertyTypeResult,
                command.PreferParking,
                heatingTypeResult.Value,
                conditionResult.Value
            );

            if (searchCriteriaResult.IsFailure)
            {
                return Result.Failure(searchCriteriaResult.Error);
            }

            var buyer = buyerResult.Value;
            buyer.UpdateSearchCriteria(searchCriteriaResult.Value);

            return await _buyerRepository.UpdateAsync(buyer);
        }
    }
}