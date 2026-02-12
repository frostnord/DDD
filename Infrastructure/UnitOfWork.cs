using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;

namespace Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    private IDealRepository? _dealRepository;
    private IPropertyRepository? _propertyRepository;
    private IClientRepository? _clientRepository;
    private IBuyerRepository? _buyerRepository;
    private ISellerRepository? _sellerRepository;
    private ICompletedDealRepository? _completedDealRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IDealRepository Deals => _dealRepository ??= new DealRepository(_context);
    public IPropertyRepository Properties => _propertyRepository ??= new PropertyRepository(_context);
    public IClientRepository Clients => _clientRepository ??= new ClientRepository(_context);
    public IBuyerRepository Buyers => _buyerRepository ??= new BuyerRepository(_context);
    public ISellerRepository Sellers => _sellerRepository ??= new SellerRepository(_context);
    public ICompletedDealRepository CompletedDeals => _completedDealRepository ??= new CompletedDealRepository(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await action(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await action(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
