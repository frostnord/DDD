using System.Collections.Generic;
using UseCases.UseCases.DTO.Deal;

namespace UseCases.Deal.Queries.SearchDealsQuery;

public sealed record SearchDealsQueryResponse(
    IEnumerable<DealDto> Items,
    int TotalCount,
    int PageSize,
    int TotalPages
);
