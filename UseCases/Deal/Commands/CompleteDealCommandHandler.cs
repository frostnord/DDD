using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Deal.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Deal.Commands;

public class CompleteDealCommandHandler : ICommandHandler<CompleteDealCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompleteDealCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(CompleteDealCommand command)
    {
        var dealId = DealId.Create(command.DealId);
        if (dealId.IsFailure)
        {
            return Result.Failure(dealId.Error);
        }

        var dealResult = await _unitOfWork.Deals.GetByIdAsync(dealId.Value);
        if (dealResult.IsFailure)
        {
            return Result.Failure(dealResult.Error);
        }

        var deal = dealResult.Value;
        if (!deal.Status.CanTransitionTo(DealStatus.Completed))
        {
            return Result.Failure($"Cannot transition deal from {deal.Status.Name} to Completed status");
        }

        deal.Complete();

        var updateResult = _unitOfWork.Deals.Update(deal);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
