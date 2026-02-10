using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
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

    public async Task<Result<Guid>> HandleAsync(CreateDealCommand command)
    {
        // Создаем ValueObject из простых типов данных
        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid client ID: {command.ClientId}");
        }

        var propertyIdResult = PropertyId.Create(command.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<Guid>($"Invalid property ID: {command.PropertyId}");
        }

        BookingId? bookingId = null;
        if (command.BookingId.HasValue)
        {
            var bookingIdResult = BookingId.Create(command.BookingId.Value);
            if (bookingIdResult.IsFailure)
            {
                return Result.Failure<Guid>($"Invalid booking ID: {command.BookingId.Value}");
            }
            bookingId = bookingIdResult.Value;
        }

        return await _unitOfWork.ExecuteInTransactionAsync(async _ =>
        {
            // Проверяем, существует ли клиент
            var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Guid>($"Client with ID {command.ClientId} does not exist");
            }

            // Проверяем, существует ли недвижимость
            var propertyResult = await _unitOfWork.Properties.GetByIdForUpdateAsync(propertyIdResult.Value);
            if (propertyResult.IsFailure)
            {
                return Result.Failure<Guid>($"Property with ID {command.PropertyId} does not exist");
            }

            // Проверяем статус недвижимости
            var property = propertyResult.Value;
            if (property.Status != PropertyStatus.ForSale)
            {
                return Result.Failure<Guid>($"Property with ID {command.PropertyId} is not available for sale");
            }

            // Создаем сделку
            var dealResult = DealEntity.Create(
                clientIdResult.Value,
                propertyIdResult.Value,
                bookingId,
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
        });
    }
}
