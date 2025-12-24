using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<Result<ClientEntity>> GetByIdAsync(ClientId id);
        Task<Result<IEnumerable<ClientEntity>>> GetAllAsync();
        Task<Result<ClientEntity>> AddAsync(ClientEntity clientEntity);
        Task<Result> UpdateAsync(ClientEntity clientEntity);
        Task<Result> DeleteAsync(ClientId id);
        Task<bool> ExistsAsync(ClientId id);
    }
}