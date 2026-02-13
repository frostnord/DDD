using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.Booking;

namespace UseCases.Reservation.Queries.SearchReservationQuery;

public sealed record SearchReservationQuery(
    Guid? ClientId,
    Guid? PropertyId
) : IQuery<Result<SearchBookingsQueryResponse>>;

public class SearchReservationQueryHandler : IQueryHandler<SearchReservationQuery, Result<SearchBookingsQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchReservationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SearchBookingsQueryResponse>> HandleAsync(SearchReservationQuery query, CancellationToken cancellationToken = default)
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

            var holdsResult = await _unitOfWork.Properties.GetActiveReservationByClientIdAsync(clientIdResult.Value, nowUtc, cancellationToken);
            if (holdsResult.IsFailure)
            {
                return Result.Failure<SearchBookingsQueryResponse>(holdsResult.Error);
            }

            var dtos = holdsResult.Value
                .Select(p =>
                {
                    p.RefreshHoldState(nowUtc);

                    return new ReservationDto(
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

        var holdResult = await _unitOfWork.Properties.GetActiveReservationByPropertyIdAsync(propertyIdResult.Value, nowUtc, cancellationToken);
        if (holdResult.IsFailure)
        {
            return Result.Failure<SearchBookingsQueryResponse>(holdResult.Error);
        }

        var items = new List<ReservationDto>();
        if (holdResult.Value != null)
        {
            var p = holdResult.Value;
            p.RefreshHoldState(nowUtc);

            if (p.ReservedByClientId != null && p.ReservedUntil != null && p.ReservedUntil.Value > nowUtc)
            {
                items.Add(new ReservationDto(
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