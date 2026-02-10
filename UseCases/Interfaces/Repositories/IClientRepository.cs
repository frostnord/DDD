using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;

namespace UseCases.Interfaces.Repositories;

public interface IClientRepository
{
    Task<Result<ClientEntity>> GetByIdAsync(ClientId id);
    Task<Result<IEnumerable<ClientEntity>>> GetAllAsync();
    Result<ClientEntity> Add(ClientEntity clientEntity);
    Result Update(ClientEntity clientEntity);
    Result Delete(ClientId id);
    Task<bool> ExistsAsync(ClientId id);
}
