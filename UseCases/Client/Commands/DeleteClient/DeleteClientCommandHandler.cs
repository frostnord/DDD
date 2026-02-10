using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Client.Commands.DeleteClient;

public class DeleteClientCommandHandler : ICommandHandler<DeleteClientCommand, ClientEntity>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClientCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClientEntity>> HandleAsync(DeleteClientCommand command)
    {
        var clientId = ClientId.Create(command.ClientId);
        if (clientId.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Invalid client ID: {clientId.Error}");
        }

        var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientId.Value);
        if (clientResult.IsFailure)
        {
            return Result.Failure<ClientEntity>($"Client with ID {command.ClientId} does not exist");
        }

        var deleteResult = _unitOfWork.Clients.Delete(clientId.Value);
        if (deleteResult.IsFailure)
        {
            return Result.Failure<ClientEntity>(deleteResult.Error);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success(clientResult.Value);
    }
}
