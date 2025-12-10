using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;

namespace UseCases.Interfaces
{
    public interface IClientService
    {
        Task<Result<Client>> CreateClientAsync(string firstName, string lastName, string email, string phoneNumber);
        Task<Result<Client>> UpdateClientAsync(Guid clientId, string firstName, string lastName, string email, string phoneNumber);
        Task<Result> DeleteClientAsync(Guid clientId);
        Task<Result<Client>> GetClientByIdAsync(Guid clientId);
        Task<Result<IEnumerable<Client>>> GetAllClientsAsync();
    }
}