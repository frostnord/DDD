using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using Domain.ValueObjects;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal
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