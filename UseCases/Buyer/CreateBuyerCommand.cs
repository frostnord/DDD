using Domain.Customers.Buyer;
using Domain.Customers.Client.VO;
using UseCases.Interfaces.Commands;

namespace UseCases.Buyer
{
    public class CreateBuyerCommand : ICommand<BuyerEntity>
    {
        public ClientId ClientId { get; set; }
        public ClientSearchCriteria SearchCriteria { get; set; }
    }
}