using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateClientCommandHandler : ICommandHandler<CreateClientCommand, Client>
    {
        private readonly IClientRepository _clientRepository;

        public CreateClientCommandHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Result<Client>> HandleAsync(CreateClientCommand command)
        {
            var clientResult = Client.Create(
                command.FirstName,
                command.LastName,
                command.ContactInfo
            );

            if (clientResult.IsFailure)
            {
                return Result.Failure<Client>(clientResult.Error);
            }

            var saveResult = await _clientRepository.AddAsync(clientResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Client>(saveResult.Error);
            }

            return Result.Success(clientResult.Value);
        }
    }
}