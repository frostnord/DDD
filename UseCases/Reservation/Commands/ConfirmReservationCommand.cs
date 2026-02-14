using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using System;
using System.Threading;
using System.Threading.Tasks;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Reservation.Commands;

public sealed record ConfirmReservationCommand(Guid PropertyId, Guid ClientId) : ICommand;

public class ConfirmReservationCommandHandler : ICommandHandler<ConfirmReservationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ConfirmReservationCommand command, CancellationToken cancellationToken = default)
    {
        var propertyIdResult = PropertyId.Create(command.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure($"Invalid property ID: {propertyIdResult.Error}");
        }

        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure($"Invalid client ID: {clientIdResult.Error}");
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async (innerCancellationToken) =>
        {
            var propertyResult = await _unitOfWork.Properties.GetByIdForUpdateAsync(propertyIdResult.Value, innerCancellationToken);
            if (propertyResult.IsFailure)
            {
                return Result.Failure(propertyResult.Error);
            }

            var property = propertyResult.Value;
            var nowUtc = DateTime.UtcNow;
            property.RefreshHoldState(nowUtc);

            if (property.ReservedUntil == null || property.ReservedByClientId != clientIdResult.Value)
            {
                return Result.Failure("Hold not found or does not belong to client");
            }

            if (property.ReservedUntil.Value <= nowUtc)
            {
                return Result.Failure("Hold already expired");
            }

            return Result.Success();
        }, cancellationToken);
    }
}