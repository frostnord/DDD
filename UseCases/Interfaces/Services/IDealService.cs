using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;

namespace UseCases.Interfaces.Services
{
    public interface IDealService
    {
        Task<Result<DealEntity>> GetByIdAsync(Guid dealId);
        Task<Result<IEnumerable<DealEntity>>> GetByClientIdAsync(Guid clientId);
        Task<Result<IEnumerable<DealEntity>>> GetByPropertyIdAsync(Guid propertyId);
        Task<Result<IEnumerable<DealEntity>>> GetAllAsync();
        Task<Result<DealEntity>> CreateAsync(Guid clientId, Guid propertyId, Guid? bookingId, DealDetails details);
        Task<Result> ConfirmAsync(Guid dealId);
        Task<Result> CompleteAsync(Guid dealId);
        Task<Result> CancelAsync(Guid dealId);
        Task<Result> UpdateAsync(DealEntity dealEntity);
        Task<Result> DeleteAsync(Guid dealId);
        Task<bool> ExistsAsync(Guid dealId);
    }
}