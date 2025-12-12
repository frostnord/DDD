using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Domain.ValueObjects;

namespace UseCases.Clients.Commands
{
    public class CreateCompleteDealCommand : ICommand<CompletedDeal>
    {
        public ClientId BuyerClientId { get; set; }
        public ClientId SellerClientId { get; set; }
        public PropertyId PropertyId { get; set; }
        public DateTime DealDate { get; set; }
        public Price DealAmount { get; set; }
        public DealType DealType { get; set; }
        public DateTime CompletionDate { get; set; }
    }
}