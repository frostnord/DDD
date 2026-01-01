using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;

namespace UseCases.Client.Queries.GetClientById
{
    public class GetClientByIdQueryHandler : IQueryHandler<GetClientByIdQuery, Result<ClientEntity>>
    {
        private readonly IClientRepository _clientRepository;

        public GetClientByIdQueryHandler(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Result<ClientEntity>> HandleAsync(GetClientByIdQuery query)
        {
            var clientIdVO = ClientId.Create(query.ClientId);
            if (clientIdVO.IsFailure)
                return Result.Failure<ClientEntity>($"Invalid client ID: {clientIdVO.Error}");
                
            var client = await _clientRepository.GetByIdAsync(clientIdVO.Value);
            if (client.IsFailure)
                return Result.Failure<ClientEntity>($"Client with ID {query.ClientId} not found");
                
            return client;
        }
    }
}