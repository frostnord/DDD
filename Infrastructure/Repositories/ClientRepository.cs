using System.Collections.Generic;
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

        public async Task<Result<ClientEntity>> GetByIdAsync(ClientId id)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == id);

            return client != null
                ? Result.Success(client)
                : Result.Failure<ClientEntity>($"Client with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<ClientEntity>>> GetAllAsync()
        {
            var clients = await _context.Clients.ToListAsync();
            return Result.Success<IEnumerable<ClientEntity>>(clients);
        }

        public async Task<Result<ClientEntity>> AddAsync(ClientEntity clientEntity)
        {
            await _context.Clients.AddAsync(clientEntity);
            await _context.SaveChangesAsync();
            return Result.Success(clientEntity);
        }

        public async Task<Result> UpdateAsync(ClientEntity clientEntity)
        {
            _context.Clients.Update(clientEntity);
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