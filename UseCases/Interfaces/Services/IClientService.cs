using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;

namespace UseCases.Interfaces.Services;

public interface IClientService
{
    Task<Result<ClientEntity>> CreateClientAsync(string firstName, string lastName, string email, string phoneNumber);

    Task<Result<ClientEntity>> UpdateClientAsync(Guid clientId, string firstName, string lastName, string email,
        string phoneNumber);

    Task<Result> DeleteClientAsync(Guid clientId);
    Task<Result<ClientEntity>> GetClientByIdAsync(Guid clientId);
    Task<Result<IEnumerable<ClientEntity>>> GetAllClientsAsync();
}