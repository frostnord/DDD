using Domain.Domain.Booking.VO;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Deal;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace UseCases.Commands
{
    public class CreateDealCommand : ICommand<Deal>
    {
        public ClientId ClientId { get; set; }
        public PropertyId PropertyId { get; set; }
        public BookingId? BookingId { get; set; }
        public DealDetails Details { get; set; }
    }
}