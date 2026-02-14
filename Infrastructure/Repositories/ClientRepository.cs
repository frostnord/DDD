using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client;
using Domain.Customers.Client.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ClientEntity>> GetByIdAsync(ClientId id, CancellationToken cancellationToken = default)
        {
            var client = await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            return client != null
                ? Result.Success(client)
                : Result.Failure<ClientEntity>($"Client with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<ClientEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var clients = await _context.Clients.AsNoTracking().ToListAsync(cancellationToken);
            return Result.Success<IEnumerable<ClientEntity>>(clients);
        }

        public Result<ClientEntity> Add(ClientEntity clientEntity)
        {
            _context.Clients.Add(clientEntity);
            return Result.Success(clientEntity);
        }

        public Result Update(ClientEntity clientEntity)
        {
            _context.Clients.Update(clientEntity);
            return Result.Success();
        }

        public Result Delete(ClientId id)
        {
            var client = _context.Clients.FirstOrDefault(c => c.Id == id);
            if (client == null)
            {
                return Result.Failure($"Client with ID {id.Value} not found");
            }

            _context.Clients.Remove(client);
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(ClientId id, CancellationToken cancellationToken = default)
        {
            return await _context.Clients.AsNoTracking().AnyAsync(c => c.Id == id, cancellationToken);
        }
    }
}
