using System;
using System.Collections.Generic;
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

        var nowUtc = DateTime.UtcNow;

        if (query.ClientId != null)
        {
            var clientIdResult = ClientId.Create(query.ClientId.Value);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<SearchBookingsQueryResponse>(clientIdResult.Error);
            }

            var holdsResult = await _unitOfWork.Properties.GetActiveHoldsByClientIdAsync(clientIdResult.Value, nowUtc);
            if (holdsResult.IsFailure)
            {
                return Result.Failure<SearchBookingsQueryResponse>(holdsResult.Error);
            }

            var dtos = holdsResult.Value
                .Select(p =>
                {
                    p.RefreshHoldState(nowUtc);

                    return new BookingDto(
                        p.Id.Value,
                        p.ReservedByClientId!.Value,
                        p.Id.Value,
                        p.ReservedAt ?? nowUtc,
                        p.ReservedUntil!.Value,
                        "Active",
                        p.CreatedAt,
                        p.UpdatedAt);
                })
                .ToList();

            return Result.Success(new SearchBookingsQueryResponse(dtos));
        }

        var propertyIdResult = PropertyId.Create(query.PropertyId!.Value);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<SearchBookingsQueryResponse>(propertyIdResult.Error);
        }

        var holdResult = await _unitOfWork.Properties.GetActiveHoldByPropertyIdAsync(propertyIdResult.Value, nowUtc);
        if (holdResult.IsFailure)
        {
            return Result.Failure<SearchBookingsQueryResponse>(holdResult.Error);
        }

        var items = new List<BookingDto>();
        if (holdResult.Value != null)
        {
            var p = holdResult.Value;
            p.RefreshHoldState(nowUtc);

            if (p.ReservedByClientId != null && p.ReservedUntil != null && p.ReservedUntil.Value > nowUtc)
            {
                items.Add(new BookingDto(
                    p.Id.Value,
                    p.ReservedByClientId.Value,
                    p.Id.Value,
                    p.ReservedAt ?? nowUtc,
                    p.ReservedUntil.Value,
                    "Active",
                    p.CreatedAt,
                    p.UpdatedAt));
            }
        }

        return Result.Success(new SearchBookingsQueryResponse(items));
    }
}
