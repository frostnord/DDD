using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.ValueObjects;

namespace UseCases.Commands
{
    public class CreateClientCommand : ICommand<Client>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}