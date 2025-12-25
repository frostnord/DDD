using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;

namespace UseCases.Interfaces.Services;

public interface ICompletedDealService
{
    Task<Result<CompletedDealEntity>> GetByIdAsync(Guid completedDealId);

    Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(Guid clientId);

    Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(Guid propertyId);

    Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync();

    Task<Result> DeleteAsync(Guid completedDealId);
}
