using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Deal;
using Domain.Domain.Property.Property.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace UseCases.Commands
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