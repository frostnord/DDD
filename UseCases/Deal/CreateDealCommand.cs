using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal
{
    public class CreateDealCommand : ICommand<DealEntity>
    {
        public Guid ClientId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid? BookingId { get; set; }
        public DealDetails Details { get; set; }
    }
}