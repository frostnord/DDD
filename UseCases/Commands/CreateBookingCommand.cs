using Domain.Domain;
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