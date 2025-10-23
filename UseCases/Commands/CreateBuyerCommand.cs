using Domain.Domain;
using Domain.Domain.ValueObjects;
using Domain.ValueObjects;

namespace UseCases.Commands
{
    public class CreateBuyerCommand : ICommand<Buyer>
    {
        public ClientId ClientId { get; set; }
        public ClientSearchCriteria SearchCriteria { get; set; }
    }
}