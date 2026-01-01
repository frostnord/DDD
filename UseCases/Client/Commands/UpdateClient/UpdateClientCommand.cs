using Domain.Customers.Client;
using UseCases.Interfaces.Commands;

namespace UseCases.Client.Commands.UpdateClient
{
    public sealed record UpdateClientCommand(
        Guid ClientId,
        string FirstName,
        string LastName,
        string Email,
        string PhoneNumber
    ) : ICommand<ClientEntity>;
}