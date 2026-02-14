using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;

namespace UseCases.Client.Queries.GetAllClient;

public class GetAllClientsQueryHandler : IQueryHandler<GetAllClientsQuery, Result<IEnumerable<ClientEntity>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllClientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<ClientEntity>>> HandleAsync(GetAllClientsQuery query, CancellationToken cancellationToken = default)
    {
        var clients = await _unitOfWork.Clients.GetAllAsync(cancellationToken);
        if (clients.IsFailure)
            return Result.Failure<IEnumerable<ClientEntity>>(clients.Error);
            
        return clients;
    }
}