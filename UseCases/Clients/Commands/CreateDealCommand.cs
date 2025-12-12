using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;

namespace UseCases.Clients.Commands
{
    public class CreateDealCommand : ICommand<DealEntity>
    {
        public ClientId ClientId { get; set; }
        public PropertyId PropertyId { get; set; }
        public BookingId? BookingId { get; set; }
        public DealDetails Details { get; set; }
    }
}