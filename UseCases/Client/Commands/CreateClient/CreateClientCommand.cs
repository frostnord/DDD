using Domain.Customers.Client;
using UseCases.Interfaces.Commands;

namespace UseCases.Client.Commands.CreateClient
{
    public sealed record CreateClientCommand(
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber
    ) : ICommand<ClientEntity>;
}