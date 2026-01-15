using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using UseCases.Interfaces.Queries;
using UseCases.Interfaces.Repositories;
using UseCases.UseCases.DTO.Deal;

namespace UseCases.Deal.Queries.GetDealById;

public class GetDealByIdQueryHandler : IQueryHandler<GetDealByIdQuery, Result<DealDto>>
{
    private readonly IDealRepository _dealRepository;

    public GetDealByIdQueryHandler(IDealRepository dealRepository)
    {
        _dealRepository = dealRepository;
    }

    public async Task<Result<DealDto>> HandleAsync(GetDealByIdQuery query)
    {
        var dealIdResult = DealId.Create(query.DealId);
        if (dealIdResult.IsFailure)
        {
            return Result.Failure<DealDto>(dealIdResult.Error);
        }

        var dealResult = await _dealRepository.GetByIdAsync(dealIdResult.Value);
        if (dealResult.IsFailure)
        {
            return Result.Failure<DealDto>(dealResult.Error);
        }

        var entity = dealResult.Value;
        var dto = new DealDto(
            entity.Id.Value,
            entity.ClientId.Value,
            entity.PropertyId.Value,
            entity.BookingId?.Value,
            entity.Details,
            entity.Status.Name,
            entity.CreatedAt,
            entity.UpdatedAt);

        return Result.Success(dto);
    }
}
