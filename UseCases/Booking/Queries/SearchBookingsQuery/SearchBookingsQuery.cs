using System;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;

namespace UseCases.Booking.Queries.SearchBookingsQuery
{
    public sealed record SearchBookingsQuery(
        Guid? ClientId,
        Guid? PropertyId
    ) : IQuery<Result<SearchBookingsQueryResponse>>;
}
