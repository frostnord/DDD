using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetCompletedDealsByClientId;

public sealed record GetCompletedDealsByClientIdQuery(Guid ClientId)
    : IQuery<Result<IEnumerable<CompletedDealDto>>>;
