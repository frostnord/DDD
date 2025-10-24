using Domain.Domain;
using Domain.Domain.Booking;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace UseCases.Commands
{
    public class CreateBookingCommand : ICommand<Booking>
    {
        public ClientId ClientId { get; set; }
        public PropertyId PropertyId { get; set; }
        public DateTime VisitDate { get; set; }
    }
}