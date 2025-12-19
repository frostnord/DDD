using System;
using Domain.Deal;
using UseCases.Interfaces.Commands;

namespace UseCases.CompleteDeal
{
    public class CreateCompleteDealCommand : ICommand<CompletedDealEntity>
    {
        public Guid BuyerClientId { get; set; }
        public Guid SellerClientId { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime DealDate { get; set; }
        public decimal DealAmount { get; set; }
        public string DealType { get; set; } = string.Empty;
        public DateTime CompletionDate { get; set; }
    }
}
