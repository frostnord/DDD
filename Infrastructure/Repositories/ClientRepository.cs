using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client;
using Domain.Domain.Customers.Client.VO;
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

        public async Task<Result<Client>> GetByIdAsync(ClientId id)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id);

            return client != null
                ? Result.Success(client)
                : Result.Failure<Client>($"Client with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<Client>>> GetAllAsync()
        {
            var clients = await _context.Clients.ToListAsync();
            return Result.Success<IEnumerable<Client>>(clients);
        }

        public async Task<Result<Client>> AddAsync(Client client)
        {
            await _context.Clients.AddAsync(client);
            await _context.SaveChangesAsync();
            return Result.Success(client);
        }

        public async Task<Result> UpdateAsync(Client client)
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(ClientId id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
            {
                return Result.Failure($"Client with ID {id.Value} not found");
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<bool> ExistsAsync(ClientId id)
        {
            return await _context.Clients.AnyAsync(c => c.Id == id);
        }
    }
}