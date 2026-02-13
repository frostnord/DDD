using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using Domain.Deal.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.CompleteDeal.Commands.DeleteCompletedDeal;

public class DeleteCompletedDealCommandHandler : ICommandHandler<DeleteCompletedDealCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCompletedDealCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteCompletedDealCommand command, CancellationToken cancellationToken = default)
    {
        var idResult = CompletedDealId.Create(command.CompletedDealId);
        if (idResult.IsFailure)
        {
            return Result.Failure(idResult.Error);
        }

        var deleteResult = _unitOfWork.CompletedDeals.Delete(idResult.Value);
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
