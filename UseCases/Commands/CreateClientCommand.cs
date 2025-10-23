using Domain.Domain;
using Domain.Domain.ValueObjects;

namespace UseCases.Commands
{
    public class CreateClientCommand : ICommand<Client>
    {
        public Name FirstName { get; set; }
        public Name LastName { get; set; }
        public ContactInfo ContactInfo { get; set; }
    }
}