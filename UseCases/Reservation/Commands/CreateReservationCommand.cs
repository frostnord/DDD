using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using System;
using System.Threading;
using System.Threading.Tasks;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Reservation.Commands;

public sealed record CreateReservationCommand(
    Guid ClientId,
    Guid PropertyId
) : ICommand<Guid>;

public class CreateReservationCommandHandler : ICommandHandler<CreateReservationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateReservationCommand command, CancellationToken cancellationToken = default)
    {
        // Создаем идентификатор клиента
        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid client ID: {clientIdResult.Error}");
        }
        
        // Создаем идентификатор объекта недвижимости
        var propertyIdResult = PropertyId.Create(command.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid property ID: {propertyIdResult.Error}");
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async innerCancellationToken =>
        {
            // Проверяем, существует ли клиент
            var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientIdResult.Value, innerCancellationToken);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Guid>(
                    $"Client with ID {command.ClientId} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _unitOfWork.Properties.GetByIdForUpdateAsync(propertyIdResult.Value, innerCancellationToken);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<Guid>(
                    $"Property with ID {command.PropertyId} does not exist");
            }

            var property = propertyResult.Value;
            var nowUtc = DateTime.UtcNow;
            property.RefreshHoldState(nowUtc);

            var holdResult = property.PlaceHold(
                clientIdResult.Value,
                nowUtc,
                TimeSpan.FromDays(1));
            if (holdResult.IsFailure)
            {
                return Result.Failure<Guid>(holdResult.Error);
            }

            return Result.Success(property.Id.Value);
        }, cancellationToken);
    }
}
