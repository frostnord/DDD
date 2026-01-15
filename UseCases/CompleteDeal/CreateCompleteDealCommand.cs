using System;
using Domain.Deal;
using UseCases.Interfaces.Commands;

namespace UseCases.CompleteDeal;

public sealed record CreateCompleteDealCommand(
    Guid BuyerClientId,
    Guid SellerClientId,
    Guid PropertyId,
    DateTime DealDate,
    decimal DealAmount,
    string DealType
) : ICommand<CompletedDealEntity>;
