using System;
using CSharpFunctionalExtensions;
using Domain.Customers.Seller;
using UseCases.Interfaces.Queries;
using UseCases.DTO.Seller;

namespace UseCases.Seller.Queries;

public sealed record GetSellerByIdQuery(Guid SellerId) : IQuery<Result<SellerDto>>;