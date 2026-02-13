using CSharpFunctionalExtensions;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.Booking;

namespace UseCases.Property.Queries;

public sealed record GetPropertyReservationQuery(Guid PropertyReservationId) : IQuery<Result<ReservationDto>>;


/// <summary>
/// Получает резервацию объекта недвижимости по его уникальному идентификатору.
/// </summary>
public class GetPropertyReservationQueryHandler : IQueryHandler<GetPropertyReservationQuery, Result<ReservationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPropertyReservationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReservationDto>> HandleAsync(GetPropertyReservationQuery query, CancellationToken cancellationToken = default)
    {
        var propertyIdResult = PropertyId.Create(query.PropertyReservationId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<ReservationDto>(propertyIdResult.Error);
        }

        var propertyResult = await _unitOfWork.Properties.GetByIdAsync(propertyIdResult.Value, cancellationToken);
        if (propertyResult.IsFailure)
        {
            return Result.Failure<ReservationDto>(propertyResult.Error);
        }

        var property = propertyResult.Value;
        var nowUtc = System.DateTime.UtcNow;
        property.RefreshHoldState(nowUtc);

        if (property.ReservedUntil == null || property.ReservedByClientId == null)
        {
            return Result.Failure<ReservationDto>("Hold not found");
        }

        if (property.ReservedUntil.Value <= nowUtc)
        {
            return Result.Failure<ReservationDto>("Hold not found");
        }

        var dto = new ReservationDto(
            property.ReservedByClientId.Value,
            property.Id.Value,
            property.ReservedAt ?? nowUtc,
            property.ReservedUntil.Value,
            "Active",
            property.CreatedAt,
            property.UpdatedAt);

        return Result.Success(dto);
    }
}