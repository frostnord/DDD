using Domain.Customers.Client;
using UseCases.Interfaces.Commands;

namespace UseCases.Client.Commands.DeleteClient;

public sealed record DeleteClientCommand(
    Guid ClientId
) : ICommand<ClientEntity>;