using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;

namespace UseCases.Interfaces.Repositories;

public interface IClientRepository
{
    Task<Result<ClientEntity>> GetByIdAsync(ClientId id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ClientEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Result<ClientEntity> Add(ClientEntity clientEntity);
    Result Update(ClientEntity clientEntity);
    Result Delete(ClientId id);
    Task<bool> ExistsAsync(ClientId id, CancellationToken cancellationToken = default);
}
