using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetAllCompletedDeals;

public class GetAllCompletedDealsQueryHandler
    : IQueryHandler<GetAllCompletedDealsQuery, Result<IEnumerable<CompletedDealDto>>>
{
    private readonly ICompletedDealRepository _completedDealRepository;

    public GetAllCompletedDealsQueryHandler(ICompletedDealRepository completedDealRepository)
    {
        _completedDealRepository = completedDealRepository;
    }

    public async Task<Result<IEnumerable<CompletedDealDto>>> HandleAsync(GetAllCompletedDealsQuery query)
    {
        var dealsResult = await _completedDealRepository.GetAllAsync();
        if (dealsResult.IsFailure)
        {
            return Result.Failure<IEnumerable<CompletedDealDto>>(dealsResult.Error);
        }

        return Result.Success(dealsResult.Value.Select(MapToDto));
    }

    private static CompletedDealDto MapToDto(CompletedDealEntity entity)
    {
        return new CompletedDealDto(
            entity.Id.Value,
            entity.BuyerClientId.Value,
            entity.SellerClientId.Value,
            entity.PropertyId.Value,
            entity.DealDate,
            entity.DealAmount.Value,
            entity.DealType.Name,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
