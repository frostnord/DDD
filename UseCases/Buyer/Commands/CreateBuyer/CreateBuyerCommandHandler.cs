using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Client.VO;
using Domain.Property.VO; // Добавляем using для SmartPropertyType, HeatingType, PropertyCondition
using Domain.ValueObjects; // Добавляем using для NumberOfRooms, Floor, TotalFloors
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Buyer.Commands.CreateBuyer;

public class CreateBuyerCommandHandler : ICommandHandler<CreateBuyerCommand, Guid>
{
    private readonly IBuyerRepository _buyerRepository;
    private readonly IClientRepository _clientRepository;

    public CreateBuyerCommandHandler(
        IBuyerRepository buyerRepository,
        IClientRepository clientRepository)
    {
        _buyerRepository = buyerRepository;
        _clientRepository = clientRepository;
    }

    public async Task<Result<Guid>> HandleAsync(CreateBuyerCommand command)
    {
        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
            return Result.Failure<Guid>(clientIdResult.Error);

        var clientExists = await _clientRepository.ExistsAsync(clientIdResult.Value);
        if (!clientExists)
        {
            return Result.Failure<Guid>($"Client with ID {command.ClientId} does not exist");
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
            return Result.Failure<Guid>(combinedResult.Error);
        }
        if (propertyTypeResult == null)
        {
            return Result.Failure<Guid>("Invalid property type");
        }

        var searchCriteriaResult = ClientSearchCriteria.Create(
            numberOfRoomsResult.Value,
            floorResult.Value,
            totalFloorsResult.Value,
            propertyTypeResult, // .Value не нужен, так как FromName возвращает SmartPropertyType
            command.PreferParking,
            heatingTypeResult.Value,
            conditionResult.Value
        );

        if (searchCriteriaResult.IsFailure)
        {
            return Result.Failure<Guid>(searchCriteriaResult.Error);
        }

        var buyerResult = BuyerEntity.Create(
            clientIdResult.Value,
            searchCriteriaResult.Value
        );

        if (buyerResult.IsFailure)
        {
            return Result.Failure<Guid>(buyerResult.Error);
        }

        var saveResult = await _buyerRepository.AddAsync(buyerResult.Value);
        if (saveResult.IsFailure)
        {
            return Result.Failure<Guid>(saveResult.Error);
        }

        return Result.Success(buyerResult.Value.Id.Value);
    }
}