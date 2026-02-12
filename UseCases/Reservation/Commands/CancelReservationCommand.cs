using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Reservation.Commands;

public sealed record CancelReservationCommand(Guid PropertyId, Guid ClientId) : ICommand;

public class CancelReservationCommandHandler : ICommandHandler<CancelReservationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CancelReservationCommand command)
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

        return await _unitOfWork.ExecuteInTransactionAsync(async _ =>
        {
            var propertyResult = await _unitOfWork.Properties.GetByIdForUpdateAsync(propertyIdResult.Value);
            if (propertyResult.IsFailure)
            {
                return Result.Failure(propertyResult.Error);
            }

            var property = propertyResult.Value;
            var nowUtc = DateTime.UtcNow;
            property.RefreshHoldState(nowUtc);

            var cancelResult = property.CancelHoldByClient(clientIdResult.Value, nowUtc);
            if (cancelResult.IsFailure)
            {
                return Result.Failure(cancelResult.Error);
            }

            var updateResult = _unitOfWork.Properties.Update(property);
            if (updateResult.IsFailure)
            {
                return Result.Failure(updateResult.Error);
            }

            return Result.Success();
        });
    }
}