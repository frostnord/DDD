using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking.VO;
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
        var bookingIdResult = BookingId.Create(query.BookingId);
        if (bookingIdResult.IsFailure)
        {
            return Result.Failure<BookingDto>(bookingIdResult.Error);
        }

        var bookingResult = await _unitOfWork.Bookings.GetByIdAsync(bookingIdResult.Value);
        if (bookingResult.IsFailure)
        {
            return Result.Failure<BookingDto>(bookingResult.Error);
        }

        var entity = bookingResult.Value;
        var dto = new BookingDto(
            entity.Id.Value,
            entity.ClientId.Value,
            entity.PropertyId.Value,
            entity.BookingPeriod.StartDate,
            entity.BookingPeriod.EndDate,
            entity.TotalPrice.Value,
            entity.CreatedAt,
            entity.UpdatedAt);

        return Result.Success(dto);
    }
}
