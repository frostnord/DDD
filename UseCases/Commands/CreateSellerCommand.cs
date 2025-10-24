using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Customers.Seller;

namespace UseCases.Commands
{
    public class CreateSellerCommand : ICommand<Seller>
    {
        public ClientId ClientId { get; set; }
    }
}