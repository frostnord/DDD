using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.Buyer.Queries.SearchBuyersQuery;

namespace UseCases.Buyer.Queries.SearchBuyersQuery
{
    public sealed record SearchBuyersQuery(
        int Page = 1,
        int PageSize = 10
    ) : IQuery<Result<SearchBuyersQueryResponse>>;
}