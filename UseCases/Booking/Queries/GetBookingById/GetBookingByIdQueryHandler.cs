using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.Booking;

namespace UseCases.Booking.Queries.GetBookingById;

public class GetBookingByIdQueryHandler : IQueryHandler<GetBookingByIdQuery, Result<BookingDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBookingByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BookingDto>> HandleAsync(GetBookingByIdQuery query)
    {
        var propertyIdResult = PropertyId.Create(query.BookingId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<BookingDto>(propertyIdResult.Error);
        }

        var propertyResult = await _unitOfWork.Properties.GetByIdAsync(propertyIdResult.Value);
        if (propertyResult.IsFailure)
        {
            return Result.Failure<BookingDto>(propertyResult.Error);
        }

        var property = propertyResult.Value;
        var nowUtc = System.DateTime.UtcNow;
        property.RefreshHoldState(nowUtc);

        if (property.ReservedUntil == null || property.ReservedByClientId == null)
        {
            return Result.Failure<BookingDto>("Hold not found");
        }

        if (property.ReservedUntil.Value <= nowUtc)
        {
            return Result.Failure<BookingDto>("Hold not found");
        }

        var dto = new BookingDto(
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
