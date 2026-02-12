using System;
using System.Threading;
using System.Threading.Tasks;
using UseCases.Interfaces.Repositories;

namespace UseCases.Interfaces.Services;

public interface IUnitOfWork
{
    IDealRepository Deals { get; }
    IPropertyRepository Properties { get; }
    IClientRepository Clients { get; }
    IBuyerRepository Buyers { get; }
    ISellerRepository Sellers { get; }
    ICompletedDealRepository CompletedDeals { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default);
}
