using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Deal.VO;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Services;
using UseCases.UseCases.DTO.Deal;

namespace UseCases.Deal.Queries.GetDealById;

public class GetDealByIdQueryHandler : IQueryHandler<GetDealByIdQuery, Result<DealDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDealByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DealDto>> HandleAsync(GetDealByIdQuery query, CancellationToken cancellationToken = default)
    {
        var dealIdResult = DealId.Create(query.DealId);
        if (dealIdResult.IsFailure)
        {
            return Result.Failure<DealDto>(dealIdResult.Error);
        }

        var dealResult = await _unitOfWork.Deals.GetByIdAsync(dealIdResult.Value, cancellationToken);
        if (dealResult.IsFailure)
        {
            return Result.Failure<DealDto>(dealResult.Error);
        }

        var entity = dealResult.Value;
        var dto = new DealDto(
            entity.Id.Value,
            entity.ClientId.Value,
            entity.PropertyId.Value,
            entity.Details,
            entity.Status.Name,
            entity.CreatedAt,
            entity.UpdatedAt);

        return Result.Success(dto);
    }
}
