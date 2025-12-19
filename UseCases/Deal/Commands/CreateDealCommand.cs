using Domain.Deal;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal.Commands
{
    public class CreateDealCommand : ICommand<DealEntity>
    {
        public Guid ClientId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid? BookingId { get; set; }
        public DealDetails Details { get; set; }
    }
}