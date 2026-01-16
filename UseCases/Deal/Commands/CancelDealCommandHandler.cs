using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Deal.Commands;

public class CancelDealCommandHandler : ICommandHandler<CancelDealCommand>
{
    private readonly IDealRepository _dealRepository;

    public CancelDealCommandHandler(IDealRepository dealRepository)
    {
        _dealRepository = dealRepository;
    }

    public async Task<Result> HandleAsync(CancelDealCommand command)
    {
        var dealId = DealId.Create(command.DealId);
        if (dealId.IsFailure)
        {
            return Result.Failure(dealId.Error);
        }

        var dealResult = await _dealRepository.GetByIdAsync(dealId.Value);
        if (dealResult.IsFailure)
        {
            return Result.Failure(dealResult.Error);
        }

        var deal = dealResult.Value;
        if (!deal.Status.CanTransitionTo(DealStatus.Cancelled))
        {
            return Result.Failure($"Cannot transition deal from {deal.Status.Name} to Cancelled status");
        }

        deal.Cancel();

        var updateResult = await _dealRepository.UpdateAsync(deal);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        return Result.Success();
    }
}