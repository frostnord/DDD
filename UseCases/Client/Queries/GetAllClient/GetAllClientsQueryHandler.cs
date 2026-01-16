using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;

namespace UseCases.Client.Queries.GetAllClient;

public class GetAllClientsQueryHandler : IQueryHandler<GetAllClientsQuery, Result<IEnumerable<ClientEntity>>>
{
    private readonly IClientRepository _clientRepository;

    public GetAllClientsQueryHandler(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<Result<IEnumerable<ClientEntity>>> HandleAsync(GetAllClientsQuery query)
    {
        var clients = await _clientRepository.GetAllAsync();
        if (clients.IsFailure)
            return Result.Failure<IEnumerable<ClientEntity>>(clients.Error);
            
        return clients;
    }
}