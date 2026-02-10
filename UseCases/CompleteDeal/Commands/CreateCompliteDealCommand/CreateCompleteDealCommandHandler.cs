using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Deal.VO;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.CompleteDeal.Commands.CreateCompliteDealCommand;

public class CreateCompleteDealCommandHandler : ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompleteDealCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        var propertyIdResult = PropertyId.Create(command.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<CompletedDealEntity>(propertyIdResult.Error);
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

        return await _unitOfWork.ExecuteInTransactionAsync(async _ =>
        {
            var buyerExistsResult = await _unitOfWork.Clients.GetByIdAsync(buyerIdResult.Value);
            if (buyerExistsResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Client with ID {command.BuyerClientId} does not exist");
            }

            var buyerRoleResult = await _unitOfWork.Buyers.GetByClientIdAsync(buyerIdResult.Value);
            if (buyerRoleResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Client with ID {command.BuyerClientId} is not registered as buyer");
            }

            var sellerExistsResult = await _unitOfWork.Clients.GetByIdAsync(sellerIdResult.Value);
            if (sellerExistsResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Client with ID {command.SellerClientId} does not exist");
            }

            var sellerRoleResult = await _unitOfWork.Sellers.GetByClientIdAsync(sellerIdResult.Value);
            if (sellerRoleResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Client with ID {command.SellerClientId} is not registered as seller");
            }

            if (buyerRoleResult.Value.ClientId == sellerRoleResult.Value.ClientId)
            {
                return Result.Failure<CompletedDealEntity>("Покупатель и продавец не могут совпадать");
            }

            var propertyExistsResult = await _unitOfWork.Properties.GetByIdAsync(propertyIdResult.Value);
            if (propertyExistsResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>($"Property with ID {command.PropertyId} does not exist");
            }

            var completedDealResult = CompletedDealEntity.Create(
                buyerRoleResult.Value.Id,
                sellerRoleResult.Value.Id,
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

            var saveResult = _unitOfWork.CompletedDeals.Add(completedDealResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<CompletedDealEntity>(saveResult.Error);
            }

            return Result.Success(completedDealResult.Value);
        });
    }
}
