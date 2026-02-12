using CSharpFunctionalExtensions;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.Booking;

namespace UseCases.Reservation.Queries;

public sealed record GetReservationByIdQuery(Guid BookingId) : IQuery<Result<ReservationDto>>;

public class GetReservationByIdQueryHandler : IQueryHandler<GetReservationByIdQuery, Result<ReservationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetReservationByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReservationDto>> HandleAsync(GetReservationByIdQuery query)
    {
        var propertyIdResult = PropertyId.Create(query.BookingId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<ReservationDto>(propertyIdResult.Error);
        }

        var propertyResult = await _unitOfWork.Properties.GetByIdAsync(propertyIdResult.Value);
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
            property.Id.Value,
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