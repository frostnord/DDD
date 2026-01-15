using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Deal;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.CompleteDeal.Commands.DeleteCompletedDeal;

public class DeleteCompletedDealCommandHandler : ICommandHandler<DeleteCompletedDealCommand>
{
    private readonly ICompletedDealRepository _completedDealRepository;

    public DeleteCompletedDealCommandHandler(ICompletedDealRepository completedDealRepository)
    {
        _completedDealRepository = completedDealRepository;
    }

    public async Task<Result> HandleAsync(DeleteCompletedDealCommand command)
    {
        var idResult = CompletedDealId.Create(command.CompletedDealId);
        if (idResult.IsFailure)
        {
            return Result.Failure(idResult.Error);
        }

        return await _completedDealRepository.DeleteAsync(idResult.Value);
    }
}
