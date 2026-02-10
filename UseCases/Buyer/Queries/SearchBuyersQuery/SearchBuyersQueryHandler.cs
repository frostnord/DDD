using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UseCases.UseCases.DTO.Buyer;
using UseCases.Interfaces.Queries;
using UseCases.Buyer.Queries.SearchBuyersQuery;
using UseCases.Interfaces.Services;

namespace UseCases.Buyer.Queries.SearchBuyersQuery;

public class SearchBuyersQueryHandler : IQueryHandler<SearchBuyersQuery, Result<SearchBuyersQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchBuyersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SearchBuyersQueryResponse>> HandleAsync(SearchBuyersQuery query)
    {
        var buyersResult = await _unitOfWork.Buyers.SearchAsync(query.Page, query.PageSize);
        if (buyersResult.IsFailure)
        {
            return Result.Failure<SearchBuyersQueryResponse>(buyersResult.Error);
        }

        var (buyers, totalCount) = buyersResult.Value;
        var pageSize = query.PageSize < 1 ? 1 : query.PageSize;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        // Используем DateTime.UtcNow, так как RegistrationDate отсутствует в BuyerEntity
        var dtos = buyers.Select(b => new BuyerDto(b.Id.Value, b.ClientId.Value, DateTime.UtcNow)).ToList();

        var response = new SearchBuyersQueryResponse(dtos, totalCount, pageSize, totalPages);

        return Result.Success(response);
    }
}
