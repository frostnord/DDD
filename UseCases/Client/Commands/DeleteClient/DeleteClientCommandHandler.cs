using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Client.Commands.DeleteClient;

public class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand, ClientEntity>
{
    private readonly IClientRepository _clientRepository;

    public DeleteClientCommandHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result<ClientEntity>> HandleAsync(DeleteClientCommand command)
    {
        var clientId = ClientId.Create(command.ClientId);
        if (clientId.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid client ID: {clientId.Error}");
        }

        var clientResult = await _clientRepository.GetByIdAsync(clientId.Value);
        if (clientResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Client with ID {command.ClientId} does not exist");
        }

        var deleteResult = await _clientRepository.DeleteAsync(clientId.Value);
        if (deleteResult.IsFailure)
        {
            return Result.Failure<ClientEntity>(deleteResult.Error);
        }

        return Result.Success(clientResult.Value);
    }
}