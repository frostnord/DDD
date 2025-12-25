using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;

namespace UseCases.Services;

public class CompletedDealService : ICompletedDealService
{
    private readonly ICompletedDealRepository _completedDealRepository;

    public CompletedDealService(ICompletedDealRepository completedDealRepository)
    {
        _completedDealRepository = completedDealRepository;
    }

    public async Task<Result<CompletedDealEntity>> GetByIdAsync(Guid completedDealId)
    {
        var idResult = CompletedDealId.Create(completedDealId);
        if (idResult.IsFailure)
        {
            return Result.Failure<CompletedDealEntity>(idResult.Error);
        }

        return await _completedDealRepository.GetByIdAsync(idResult.Value);
    }

    public async Task<Result<IEnumerable<CompletedDealEntity>>> GetByClientIdAsync(Guid clientId)
    {
        var clientIdResult = ClientId.Create(clientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure<IEnumerable<CompletedDealEntity>>(clientIdResult.Error);
        }

        return await _completedDealRepository.GetByClientIdAsync(clientIdResult.Value);
    }

    public async Task<Result<IEnumerable<CompletedDealEntity>>> GetByPropertyIdAsync(Guid propertyId)
    {
        var propertyIdResult = PropertyId.Create(propertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<IEnumerable<CompletedDealEntity>>(propertyIdResult.Error);
        }

        return await _completedDealRepository.GetByPropertyIdAsync(propertyIdResult.Value);
    }

    public async Task<Result<IEnumerable<CompletedDealEntity>>> GetAllAsync()
    {
        return await _completedDealRepository.GetAllAsync();
    }

    public async Task<Result> DeleteAsync(Guid completedDealId)
    {
        var idResult = CompletedDealId.Create(completedDealId);
        if (idResult.IsFailure)
        {
            return Result.Failure(idResult.Error);
        }

        return await _completedDealRepository.DeleteAsync(idResult.Value);
    }
}
