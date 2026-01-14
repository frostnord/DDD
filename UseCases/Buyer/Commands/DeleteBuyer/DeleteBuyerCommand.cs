using Domain.Customers.Buyer;
using UseCases.Interfaces.Commands;

namespace UseCases.Buyer.Commands.DeleteBuyer
{
    public sealed record DeleteBuyerCommand(Guid BuyerId) : ICommand<bool>;
}