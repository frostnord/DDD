using System;
using CSharpFunctionalExtensions;
using UseCases.UseCases.DTO.Buyer;
using UseCases.Interfaces.Queries;

namespace UseCases.Buyer.Queries.GetBuyerById;

public sealed record GetBuyerByIdQuery(Guid BuyerId) : IQuery<Result<BuyerDto>>;