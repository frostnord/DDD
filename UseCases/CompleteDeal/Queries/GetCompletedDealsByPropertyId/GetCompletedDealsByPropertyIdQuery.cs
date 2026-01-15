using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetCompletedDealsByPropertyId;

public sealed record GetCompletedDealsByPropertyIdQuery(Guid PropertyId)
    : IQuery<Result<IEnumerable<CompletedDealDto>>>;
