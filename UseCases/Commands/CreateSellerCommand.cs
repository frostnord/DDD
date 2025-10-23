using Domain.Domain;
using Domain.Domain.ValueObjects;

namespace UseCases.Commands
{
    public class CreateSellerCommand : ICommand<Seller>
    {
        public ClientId ClientId { get; set; }
    }
}