using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Booking.Commands.ConfirmBooking;

public class ConfirmBookingCommandHandler : ICommandHandler<ConfirmBookingCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmBookingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ConfirmBookingCommand command)
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

            if (property.ReservedUntil == null || property.ReservedByClientId != clientIdResult.Value)
            {
                return Result.Failure("Hold not found or does not belong to client");
            }

            if (property.ReservedUntil.Value <= nowUtc)
            {
                return Result.Failure("Hold already expired");
            }

            return Result.Success();
        });
    }
}
