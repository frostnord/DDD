using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using UseCases.Interfaces.Queries;

namespace UseCases.Client.Queries.GetClientById;

public sealed record GetClientByIdQuery(Guid ClientId) : IQuery<Result<ClientEntity>>;