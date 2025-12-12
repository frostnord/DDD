using Domain.Customers.Seller;
using UseCases.Interfaces.Commands;

namespace UseCases.Seller
{
    public class CreateSellerCommand : ICommand<SellerEntity>
    {
        public Guid ClientId { get; set; }
    }
}