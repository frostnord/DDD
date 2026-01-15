using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetCompletedDealsByPropertyId;

public class GetCompletedDealsByPropertyIdQueryHandler
    : IQueryHandler<GetCompletedDealsByPropertyIdQuery, Result<IEnumerable<CompletedDealDto>>>
{
    private readonly ICompletedDealRepository _completedDealRepository;

    public GetCompletedDealsByPropertyIdQueryHandler(ICompletedDealRepository completedDealRepository)
    {
        _completedDealRepository = completedDealRepository;
    }

    public async Task<Result<IEnumerable<CompletedDealDto>>> HandleAsync(GetCompletedDealsByPropertyIdQuery query)
    {
        var propertyIdResult = PropertyId.Create(query.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<IEnumerable<CompletedDealDto>>(propertyIdResult.Error);
        }

        var dealsResult = await _completedDealRepository.GetByPropertyIdAsync(propertyIdResult.Value);
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
