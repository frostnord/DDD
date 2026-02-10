using System;
using System.Collections.Generic;
using System.Linq;
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

    public async Task<Result<SearchSellersQueryResponse>> HandleAsync(SearchSellersQuery query)
    {
        var sellersResult = await _unitOfWork.Sellers.GetAllAsync();
        if (sellersResult.IsFailure)
        {
            return Result.Failure<SearchSellersQueryResponse>(sellersResult.Error);
        }

        var sellers = sellersResult.Value.ToList();
        var totalCount = sellers.Count;

        // Применение сортировки
        var sortedSellers = ApplySorting(sellers, query.SortBy, query.SortOrder);

        // Применение пагинации
        var pagedSellers = sortedSellers
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var sellerDtos = pagedSellers.Select(seller => new SellerDto(
            seller.Id.Value,
            seller.ClientId.Value,
            seller.RegisteredAt
        )).ToList();

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        var response = new SearchSellersQueryResponse(
            sellerDtos,
            totalCount,
            query.PageSize,
            totalPages,
            query.Page
        );

        return Result.Success(response);
    }

    private List<Domain.Customers.Seller.SellerEntity> ApplySorting(
        List<Domain.Customers.Seller.SellerEntity> sellers,
        string sortBy,
        string sortOrder)
    {
        var sortedSellers = sortBy.ToLower() switch
        {
            "id" => sortOrder.ToLower() == "desc" 
                ? sellers.OrderByDescending(s => s.Id.Value).ToList()
                : sellers.OrderBy(s => s.Id.Value).ToList(),
            "clientid" => sortOrder.ToLower() == "desc"
                ? sellers.OrderByDescending(s => s.ClientId.Value).ToList()
                : sellers.OrderBy(s => s.ClientId.Value).ToList(),
            _ => sellers // по умолчанию сортировка по ID
        };

        return sortedSellers;
    }
}