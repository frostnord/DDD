using Domain.Customers.Buyer;
using Domain.Customers.Client.VO;
using UseCases.Interfaces.Commands;

namespace UseCases.Buyer
{
    public sealed record CreateBuyerCommand(ClientId ClientId, ClientSearchCriteria SearchCriteria) : ICommand<BuyerEntity>;
}