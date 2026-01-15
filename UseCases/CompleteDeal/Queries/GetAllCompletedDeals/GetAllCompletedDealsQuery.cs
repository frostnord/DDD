using System.Collections.Generic;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetAllCompletedDeals;

public sealed record GetAllCompletedDealsQuery()
    : IQuery<Result<IEnumerable<CompletedDealDto>>>;
