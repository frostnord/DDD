using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.DTO.Seller;

namespace UseCases.Seller.Queries;

public class SearchSellersQueryHandler : IQueryHandler<SearchSellersQuery, Result<SearchSellersQueryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchSellersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SearchSellersQueryResponse>> HandleAsync(SearchSellersQuery query, CancellationToken cancellationToken = default)
    {
        var normalizedPage = query.Page < 1 ? 1 : query.Page;
        var normalizedPageSize = query.PageSize < 1 ? 1 : query.PageSize;

        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "Id" : query.SortBy;
        var sortOrder = string.IsNullOrWhiteSpace(query.SortOrder) ? "asc" : query.SortOrder;

        var sellersResult = await _unitOfWork.Sellers.SearchAsync(
            normalizedPage,
            normalizedPageSize,
            sortBy,
            sortOrder,
            cancellationToken);

        if (sellersResult.IsFailure)
        {
            return Result.Failure<SearchSellersQueryResponse>(sellersResult.Error);
        }

        var (items, totalCount) = sellersResult.Value;

        var sellerDtos = items.Select(seller => new SellerDto(
                seller.Id.Value,
                seller.ClientId.Value,
                seller.RegisteredAt))
            .ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / normalizedPageSize);

        var response = new SearchSellersQueryResponse(
            sellerDtos,
            totalCount,
            normalizedPageSize,
            totalPages,
            normalizedPage
        );

        return Result.Success(response);
    }
}