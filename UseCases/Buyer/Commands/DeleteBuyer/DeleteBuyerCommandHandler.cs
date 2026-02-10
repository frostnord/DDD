using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Buyer.Commands.DeleteBuyer;

public class DeleteBuyerCommandHandler : ICommandHandler<DeleteBuyerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBuyerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteBuyerCommand command)
    {
        var buyerId = BuyerId.Create(command.BuyerId);
        if (buyerId.IsFailure)
            return Result.Failure(buyerId.Error);

        var buyerResult = await _unitOfWork.Buyers.GetByIdAsync(buyerId.Value);
        if (buyerResult.IsFailure)
        {
            return Result.Failure($"Buyer with ID {command.BuyerId} does not exist");
        }

        var deleteResult = _unitOfWork.Buyers.Delete(buyerId.Value);
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
