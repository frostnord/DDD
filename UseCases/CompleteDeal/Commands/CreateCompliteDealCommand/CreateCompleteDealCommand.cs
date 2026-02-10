using Domain.Deal;
using UseCases.Interfaces.Commands;

namespace UseCases.CompleteDeal.Commands.CreateCompliteDealCommand;

public sealed record CreateCompleteDealCommand(
    Guid BuyerClientId,
    Guid SellerClientId,
    Guid PropertyId,
    DateTime DealDate,
    decimal DealAmount,
    string DealType
) : ICommand<CompletedDealEntity>;
