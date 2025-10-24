using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;

namespace UseCases.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<Result<Client>> GetByIdAsync(ClientId id);
        Task<Result<IEnumerable<Client>>> GetAllAsync();
        Task<Result<Client>> AddAsync(Client client);
        Task<Result> UpdateAsync(Client client);
        Task<Result> DeleteAsync(ClientId id);
        Task<bool> ExistsAsync(ClientId id);
    }
}