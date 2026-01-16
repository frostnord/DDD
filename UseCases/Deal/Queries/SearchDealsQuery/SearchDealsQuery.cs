using System;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;

namespace UseCases.Deal.Queries.SearchDealsQuery;

public sealed record SearchDealsQuery(
    Guid? ClientId,
    Guid? PropertyId,
    int Page = 1,
    int PageSize = 10
) : IQuery<Result<SearchDealsQueryResponse>>;
