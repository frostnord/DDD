using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Deal.Commands;

public class ConfirmDealCommandHandler : ICommandHandler<ConfirmDealCommand>
{
    private readonly IDealRepository _dealRepository;

    public ConfirmDealCommandHandler(IDealRepository dealRepository)
    {
        _dealRepository = dealRepository;
    }

    public async Task<Result> HandleAsync(ConfirmDealCommand command)
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

        // Проверяем возможность перехода в статус подтверждения
        if (!deal.Status.CanTransitionTo(DealStatus.Confirmed))
        {
            return Result.Failure($"Cannot transition deal from {deal.Status.Name} to Confirmed status");
        }

        deal.Confirm();

        var updateResult = await _dealRepository.UpdateAsync(deal);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        return Result.Success();
    }
}