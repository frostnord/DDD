using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Deal.Commands;

public class CreateDealCommandHandler : ICommandHandler<CreateDealCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateDealCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateDealCommand command, CancellationToken cancellationToken = default)
    {
        // Создаем идентификатор клиента
        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid client ID: {command.ClientId}");
        }
        // Создаем идентификатор недвижимости
        var propertyIdResult = PropertyId.Create(command.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid property ID: {command.PropertyId}");
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async innerCancellationToken =>
        {
            // Проверяем, существует ли клиент
            var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientIdResult.Value, innerCancellationToken);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Guid>($"Client with ID {command.ClientId} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _unitOfWork.Properties.GetByIdForUpdateAsync(propertyIdResult.Value, innerCancellationToken);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<Guid>($"Property with ID {command.PropertyId} does not exist");
            }

            var property = propertyResult.Value;
            var nowUtc = DateTime.UtcNow;

            property.RefreshHoldState(nowUtc);

            if (property.Status == PropertyStatus.Sold)
            {
                return Result.Failure<Guid>($"Property with ID {command.PropertyId} is already sold");
            }

            if (property.Status == PropertyStatus.Reserved)
            {
                if (property.ReservedUntil == null)
                {
                    return Result.Failure<Guid>($"Property with ID {command.PropertyId} is reserved");
                }

                if (property.ReservedByClientId != clientIdResult.Value)
                {
                    return Result.Failure<Guid>($"Property with ID {command.PropertyId} is reserved by another client until {property.ReservedUntil.Value:o}");
                }
            }

            if (property.Status == PropertyStatus.ForSale)
            {
                var holdResult = property.PlaceHold(
                    clientIdResult.Value,
                    nowUtc,
                    TimeSpan.FromMinutes(5));

                if (holdResult.IsFailure)
                {
                    return Result.Failure<Guid>(holdResult.Error);
                }

                var updateResult = _unitOfWork.Properties.Update(property);
                if (updateResult.IsFailure)
                {
                    return Result.Failure<Guid>(updateResult.Error);
                }
            }

            // Создаем сделку
            var dealResult = DealEntity.Create(
                clientIdResult.Value,
                propertyIdResult.Value,
                command.Details
            );

            if (dealResult.IsFailure)
            {
                return Result.Failure<Guid>(dealResult.Error);
            }

            // Сохраняем сделку
            var saveResult = _unitOfWork.Deals.Add(dealResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Guid>(saveResult.Error);
            }

            return Result.Success(dealResult.Value.Id.Value);
        }, cancellationToken);
    }
}
