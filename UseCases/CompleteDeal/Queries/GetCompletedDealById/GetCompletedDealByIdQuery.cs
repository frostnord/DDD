using System;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetCompletedDealById;

public sealed record GetCompletedDealByIdQuery(Guid CompletedDealId)
    : IQuery<Result<CompletedDealDto>>;
