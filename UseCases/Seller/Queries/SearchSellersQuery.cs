using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.DTO.Seller;

namespace UseCases.Seller.Queries;

public sealed record SearchSellersQuery(
    int Page = 1,
    int PageSize = 10,
    string SortBy = "Id",
    string SortOrder = "asc"
) : IQuery<Result<SearchSellersQueryResponse>>;

public sealed record SearchSellersQueryResponse(
    IEnumerable<SellerDto> Items,
    int TotalCount,
    int PageSize,
    int TotalPages,
    int CurrentPage
);