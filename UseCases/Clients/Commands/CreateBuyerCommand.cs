using Domain.Customers.Buyer;
using Domain.Customers.Client.VO;

namespace UseCases.Clients.Commands
{
    public class CreateBuyerCommand : ICommand<BuyerEntity>
    {
        public ClientId ClientId { get; set; }
        public ClientSearchCriteria SearchCriteria { get; set; }
    }
}