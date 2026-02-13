using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Deal.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.CompletedDeal;

namespace UseCases.CompleteDeal.Queries.GetCompletedDealById;

public class GetCompletedDealByIdQueryHandler
    : IQueryHandler<GetCompletedDealByIdQuery, Result<CompletedDealDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompletedDealByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompletedDealDto>> HandleAsync(GetCompletedDealByIdQuery query, CancellationToken cancellationToken = default)
    {
        var idResult = CompletedDealId.Create(query.CompletedDealId);
        if (idResult.IsFailure)
        {
            return Result.Failure<CompletedDealDto>(idResult.Error);
        }

        var dealResult = await _unitOfWork.CompletedDeals.GetByIdAsync(idResult.Value, cancellationToken);
        if (dealResult.IsFailure)
        {
            return Result.Failure<CompletedDealDto>(dealResult.Error);
        }

        return Result.Success(MapToDto(dealResult.Value));
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
