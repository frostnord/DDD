using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.CompleteDeal
{
    public class CreateCompleteDealCommandHandler : ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>
    {
        private readonly ICompletedDealRepository _completedDealRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IPropertyRepository _propertyRepository;

        public CreateCompleteDealCommandHandler(
            ICompletedDealRepository completedDealRepository,
            IClientRepository clientRepository,
            IPropertyRepository propertyRepository)
        {
            _completedDealRepository = completedDealRepository;
            _clientRepository = clientRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<Result<CompletedDealEntity>> HandleAsync(CreateCompleteDealCommand command)
        {
            var buyerIdResult = ClientId.Create(command.BuyerClientId);
            if (buyerIdResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>(buyerIdResult.Error);
            }

            var sellerIdResult = ClientId.Create(command.SellerClientId);
            if (sellerIdResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>(sellerIdResult.Error);
            }

            var buyerExistsResult = await _clientRepository.GetByIdAsync(buyerIdResult.Value);
            if (buyerExistsResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Client with ID {command.BuyerClientId} does not exist");
            }

            var sellerExistsResult = await _clientRepository.GetByIdAsync(sellerIdResult.Value);
            if (sellerExistsResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Client with ID {command.SellerClientId} does not exist");
            }

            var propertyIdResult = PropertyId.Create(command.PropertyId);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>(propertyIdResult.Error);
            }

            var propertyExistsResult = await _propertyRepository.GetByIdAsync(propertyIdResult.Value);
            if (propertyExistsResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Property with ID {command.PropertyId} does not exist");
            }

            var priceResult = Price.Create(command.DealAmount);
            if (priceResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>(priceResult.Error);
            }

            DealType dealTypeValue;
            try
            {
                dealTypeValue = DealType.FromName(command.DealType);
            }
            catch (ArgumentException)
            {
                return Result.Failure<CompletedDealEntity>($"Тип сделки '{command.DealType}' не поддерживается.");
            }

            var completedDealResult = CompletedDealEntity.Create(
                buyerIdResult.Value,
                sellerIdResult.Value,
                propertyIdResult.Value,
                command.DealDate,
                priceResult.Value,
                dealTypeValue);

            if (completedDealResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>(completedDealResult.Error);
            }

            return await _completedDealRepository.AddAsync(completedDealResult.Value);
        }
    }
}
