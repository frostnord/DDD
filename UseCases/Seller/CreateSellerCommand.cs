using Domain.Customers.Seller;
using UseCases.Interfaces.Commands;

namespace UseCases.Seller
{
    public record CreateSellerCommand(Guid ClientId) : ICommand<SellerEntity>;
}