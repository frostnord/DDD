using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using UseCases.Interfaces.Queries;

namespace UseCases.Client.Queries.GetAllClient
{
    public sealed record GetAllClientsQuery : IQuery<Result<IEnumerable<ClientEntity>>>;
}