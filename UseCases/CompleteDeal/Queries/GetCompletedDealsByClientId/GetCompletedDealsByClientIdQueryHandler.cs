using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Deal;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetCompletedDealsByClientId;

public class GetCompletedDealsByClientIdQueryHandler
    : IQueryHandler<GetCompletedDealsByClientIdQuery, Result<IEnumerable<CompletedDealDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompletedDealsByClientIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<CompletedDealDto>>> HandleAsync(GetCompletedDealsByClientIdQuery query)
    {
        var clientIdResult = ClientId.Create(query.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure<IEnumerable<CompletedDealDto>>(clientIdResult.Error);
        }

        var dealsResult = await _unitOfWork.CompletedDeals.GetByClientIdAsync(clientIdResult.Value);
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
