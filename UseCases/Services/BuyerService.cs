using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using Domain.Customers.Buyer.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;

namespace UseCases.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IClientRepository _clientRepository;

        public BuyerService(IBuyerRepository buyerRepository, IClientRepository clientRepository)
        {
            _buyerRepository = buyerRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result<BuyerEntity>> CreateBuyerAsync(Guid clientId, int preferredNumberOfRooms, int preferredFloor,
            int preferredTotalFloors, string preferredType, string preferredHeatingType, string preferredCondition,
            bool? preferParking)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(clientIdResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>($"Client with ID {clientId} does not exist");
            }

            // Создаем Value Objects из примитивных значений
            var numberOfRoomsResult = NumberOfRooms.Create(preferredNumberOfRooms);
            if (numberOfRoomsResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(numberOfRoomsResult.Error);
            }

            var floorResult = Floor.Create(preferredFloor);
            if (floorResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(floorResult.Error);
            }

            var totalFloorsResult = TotalFloors.Create(preferredTotalFloors);
            if (totalFloorsResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(totalFloorsResult.Error);
            }

            SmartPropertyType propertyTypeValue;
            try
            {
                propertyTypeValue = SmartPropertyType.FromName(preferredType);
            }
            catch (ArgumentException)
            {
                return Result.Failure<BuyerEntity>($"Тип недвижимости '{preferredType}' не поддерживается.");
            }

            var heatingTypeResult = HeatingType.Create(preferredHeatingType);
            if (heatingTypeResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(heatingTypeResult.Error);
            }

            var conditionResult = PropertyCondition.Create(preferredCondition);
            if (conditionResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(conditionResult.Error);
            }

            var searchCriteriaResult = ClientSearchCriteria.Create(
                numberOfRoomsResult.Value,
                floorResult.Value,
                totalFloorsResult.Value,
                propertyTypeValue,
                preferParking,
                heatingTypeResult.Value,
                conditionResult.Value);

            if (searchCriteriaResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(searchCriteriaResult.Error);
            }

            var buyerResult = BuyerEntity.Create(clientIdResult.Value, searchCriteriaResult.Value);
            if (buyerResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(buyerResult.Error);
            }

            var saveResult = await _buyerRepository.AddAsync(buyerResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(saveResult.Error);
            }

            return Result.Success(buyerResult.Value);
        }

        public async Task<Result<BuyerEntity>> GetBuyerByIdAsync(Guid buyerId)
        {
            var buyerIdResult = BuyerId.Create(buyerId);
            if (buyerIdResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(buyerIdResult.Error);
            }

            return await _buyerRepository.GetByIdAsync(buyerIdResult.Value);
        }

        public async Task<Result<IEnumerable<BuyerEntity>>> GetAllBuyersAsync()
        {
            return await _buyerRepository.GetAllAsync();
        }

        public async Task<Result> UpdateBuyerAsync(Guid buyerId, Guid clientId, int preferredNumberOfRooms,
            int preferredFloor, int preferredTotalFloors, string preferredType, string preferredHeatingType,
            string preferredCondition, bool? preferParking)
        {
            var buyerIdResult = BuyerId.Create(buyerId);
            if (buyerIdResult.IsFailure)
            {
                return Result.Failure(buyerIdResult.Error);
            }

            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure(clientIdResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure($"Client with ID {clientId} does not exist");
            }

            var buyerResult = await _buyerRepository.GetByIdAsync(buyerIdResult.Value);
            if (buyerResult.IsFailure)
            {
                return Result.Failure(buyerResult.Error);
            }

            // Создаем Value Objects из примитивных значений
            var numberOfRoomsResult = NumberOfRooms.Create(preferredNumberOfRooms);
            if (numberOfRoomsResult.IsFailure)
            {
                return Result.Failure(numberOfRoomsResult.Error);
            }

            var floorResult = Floor.Create(preferredFloor);
            if (floorResult.IsFailure)
            {
                return Result.Failure(floorResult.Error);
            }

            var totalFloorsResult = TotalFloors.Create(preferredTotalFloors);
            if (totalFloorsResult.IsFailure)
            {
                return Result.Failure(totalFloorsResult.Error);
            }

            SmartPropertyType propertyTypeValue;
            try
            {
                propertyTypeValue = SmartPropertyType.FromName(preferredType);
            }
            catch (ArgumentException)
            {
                return Result.Failure($"Тип недвижимости '{preferredType}' не поддерживается.");
            }

            var heatingTypeResult = HeatingType.Create(preferredHeatingType);
            if (heatingTypeResult.IsFailure)
            {
                return Result.Failure(heatingTypeResult.Error);
            }

            var conditionResult = PropertyCondition.Create(preferredCondition);
            if (conditionResult.IsFailure)
            {
                return Result.Failure(conditionResult.Error);
            }

            var searchCriteriaResult = ClientSearchCriteria.Create(
                numberOfRoomsResult.Value,
                floorResult.Value,
                totalFloorsResult.Value,
                propertyTypeValue,
                preferParking,
                heatingTypeResult.Value,
                conditionResult.Value);

            if (searchCriteriaResult.IsFailure)
            {
                return Result.Failure(searchCriteriaResult.Error);
            }

            // Обновляем критерии поиска покупателя
            buyerResult.Value.UpdateSearchCriteria(searchCriteriaResult.Value);
            var updateResult = await _buyerRepository.UpdateAsync(buyerResult.Value);
            return updateResult;
        }

        public async Task<Result> DeleteBuyerAsync(Guid buyerId)
        {
            var buyerIdResult = BuyerId.Create(buyerId);
            if (buyerIdResult.IsFailure)
            {
                return Result.Failure(buyerIdResult.Error);
            }

            var buyerResult = await _buyerRepository.GetByIdAsync(buyerIdResult.Value);
            if (buyerResult.IsFailure)
            {
                return Result.Failure(buyerResult.Error);
            }

            var deleteResult = await _buyerRepository.DeleteAsync(buyerIdResult.Value);
            return deleteResult;
        }
    }
}