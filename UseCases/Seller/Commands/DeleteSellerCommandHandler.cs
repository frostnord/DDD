using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Seller.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Seller.Commands;

public class DeleteSellerCommandHandler : ICommandHandler<DeleteSellerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSellerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(DeleteSellerCommand command, CancellationToken cancellationToken = default)
    {
        var sellerIdResult = SellerId.Create(command.SellerId);
        if (sellerIdResult.IsFailure)
        {
            return Result.Failure(sellerIdResult.Error);
        }

        var sellerResult = await _unitOfWork.Sellers.GetByIdAsync(sellerIdResult.Value, cancellationToken);
        if (sellerResult.IsFailure)
        {
            return Result.Failure(sellerResult.Error);
        }

        var deleteResult = _unitOfWork.Sellers.Delete(sellerIdResult.Value);
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
