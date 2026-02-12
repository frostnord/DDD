using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.Deal;

namespace UseCases.Deal.Queries.SearchDealsQuery;

public class SearchDealsQueryHandler : IQueryHandler<SearchDealsQuery, Result<SearchDealsQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchDealsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SearchDealsQueryResponse>> HandleAsync(SearchDealsQuery query)
    {
        if (query.ClientId == null && query.PropertyId == null)
        {
            return Result.Failure<SearchDealsQueryResponse>("Нужен id клиента или недвижимости");
        }

        Result<IEnumerable<DealEntity>> dealsResult;

        if (query.ClientId != null)
        {
            var clientIdResult = ClientId.Create(query.ClientId.Value);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<SearchDealsQueryResponse>(clientIdResult.Error);
            }

            dealsResult = await _unitOfWork.Deals.GetByClientIdAsync(clientIdResult.Value);
        }
        else
        {
            var propertyIdResult = PropertyId.Create(query.PropertyId!.Value);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure<SearchDealsQueryResponse>(propertyIdResult.Error);
            }

            dealsResult = await _unitOfWork.Deals.GetByPropertyIdAsync(propertyIdResult.Value);
        }

        if (dealsResult.IsFailure)
        {
            return Result.Failure<SearchDealsQueryResponse>(dealsResult.Error);
        }

        var allDeals = dealsResult.Value.ToList();
        var totalCount = allDeals.Count;

        var pagedDeals = allDeals
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var dtos = pagedDeals.Select(d => new DealDto(
            d.Id.Value,
            d.ClientId.Value,
            d.PropertyId.Value,
            d.Details,
            d.Status.Name,
            d.CreatedAt,
            d.UpdatedAt)).ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        return Result.Success(new SearchDealsQueryResponse(
            dtos,
            totalCount,
            query.PageSize,
            totalPages));
    }
}
