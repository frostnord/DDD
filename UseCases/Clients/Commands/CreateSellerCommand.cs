using Domain.Customers.Seller;

namespace UseCases.Clients.Commands
{
    public class CreateSellerCommand : ICommand<SellerEntity>
    {
        public Guid ClientId { get; set; }
    }
}