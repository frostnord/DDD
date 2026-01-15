using System;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.Deal;

namespace UseCases.Deal.Queries.GetDealById;

public sealed record GetDealByIdQuery(Guid DealId) : IQuery<Result<DealDto>>;
