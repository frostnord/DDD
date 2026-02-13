using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Deal.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Deal.Commands;

public class CancelDealCommandHandler : ICommandHandler<CancelDealCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelDealCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CancelDealCommand command, CancellationToken cancellationToken = default)
    {
        var dealId = DealId.Create(command.DealId);
        if (dealId.IsFailure)
        {
            return Result.Failure(dealId.Error);
        }

        var dealResult = await _unitOfWork.Deals.GetByIdAsync(dealId.Value, cancellationToken);
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

        var updateResult = _unitOfWork.Deals.Update(deal);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
