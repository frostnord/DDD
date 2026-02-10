using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Property.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetCompletedDealsByPropertyId;

public class GetCompletedDealsByPropertyIdQueryHandler
    : IQueryHandler<GetCompletedDealsByPropertyIdQuery, Result<IEnumerable<CompletedDealDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompletedDealsByPropertyIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<CompletedDealDto>>> HandleAsync(GetCompletedDealsByPropertyIdQuery query)
    {
        var propertyIdResult = PropertyId.Create(query.PropertyId);
        if (propertyIdResult.IsFailure)
        {
            return Result.Failure<IEnumerable<CompletedDealDto>>(propertyIdResult.Error);
        }

        var dealsResult = await _unitOfWork.CompletedDeals.GetByPropertyIdAsync(propertyIdResult.Value);
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
