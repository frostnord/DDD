using Domain.Domain.Customers.Buyer;
using Domain.Domain.Customers.Client.VO;

namespace UseCases.Commands
{
    public class CreateBuyerCommand : ICommand<Buyer>
    {
        public ClientId ClientId { get; set; }
        public ClientSearchCriteria SearchCriteria { get; set; }
    }
}