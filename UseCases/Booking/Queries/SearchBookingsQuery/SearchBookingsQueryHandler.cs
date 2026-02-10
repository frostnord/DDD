using System;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.Booking;

namespace UseCases.Booking.Queries.SearchBookingsQuery;

public class SearchBookingsQueryHandler : IQueryHandler<SearchBookingsQuery, Result<SearchBookingsQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchBookingsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SearchBookingsQueryResponse>> HandleAsync(SearchBookingsQuery query)
    {
        if (query.ClientId == null && query.PropertyId == null)
        {
            return Result.Failure<SearchBookingsQueryResponse>("Нужен id клиента или недвижимости");
        }

        Result<System.Collections.Generic.IEnumerable<Domain.Booking.BookingEntity>> bookingsResult;

        if (query.ClientId != null)
        {
            var clientIdResult = ClientId.Create(query.ClientId.Value);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<SearchBookingsQueryResponse>(clientIdResult.Error);
            }

            bookingsResult = await _unitOfWork.Bookings.GetByClientIdAsync(clientIdResult.Value);
        }
        else
        {
            var propertyIdResult = PropertyId.Create(query.PropertyId!.Value);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure<SearchBookingsQueryResponse>(propertyIdResult.Error);
            }

            bookingsResult = await _unitOfWork.Bookings.GetByPropertyIdAsync(propertyIdResult.Value);
        }

        if (bookingsResult.IsFailure)
        {
            return Result.Failure<SearchBookingsQueryResponse>(bookingsResult.Error);
        }

        var dtos = bookingsResult.Value.Select(b => new BookingDto(
            b.Id.Value,
            b.ClientId.Value,
            b.PropertyId.Value,
            b.BookingPeriod.StartDate,
            b.BookingPeriod.EndDate,
            b.TotalPrice.Value,
            b.CreatedAt,
            b.UpdatedAt)).ToList();

        return Result.Success(new SearchBookingsQueryResponse(dtos));
    }
}
