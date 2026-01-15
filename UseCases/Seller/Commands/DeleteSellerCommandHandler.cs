using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Seller.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Seller.Commands;

public class DeleteSellerCommandHandler : ICommandHandler<DeleteSellerCommand>
{
    private readonly ISellerRepository _sellerRepository;

    public DeleteSellerCommandHandler(ISellerRepository sellerRepository)
    {
        _sellerRepository = sellerRepository;
    }

    public async Task<Result> HandleAsync(DeleteSellerCommand command)
    {
        var sellerIdResult = SellerId.Create(command.SellerId);
        if (sellerIdResult.IsFailure)
        {
            return Result.Failure(sellerIdResult.Error);
        }

        var sellerResult = await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
        if (sellerResult.IsFailure)
        {
            return Result.Failure(sellerResult.Error);
        }

        var deleteResult = await _sellerRepository.DeleteAsync(sellerIdResult.Value);
        return deleteResult;
    }
}