using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Seller.Commands;

public class UpdateSellerCommandHandler : ICommandHandler<UpdateSellerCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSellerCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(UpdateSellerCommand command)
    {
        var sellerIdResult = SellerId.Create(command.SellerId);
        if (sellerIdResult.IsFailure)
        {
            return Result.Failure(sellerIdResult.Error);
        }

        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure(clientIdResult.Error);
        }

        var sellerResult = await _unitOfWork.Sellers.GetByIdAsync(sellerIdResult.Value);
        if (sellerResult.IsFailure)
        {
            return Result.Failure(sellerResult.Error);
        }

        var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientIdResult.Value);
        if (clientResult.IsFailure)
        {
            return Result.Failure($"Client with ID {command.ClientId} does not exist");
        }
    
        SellerEntity seller = sellerResult.Value;
        seller.Update(clientIdResult.Value);

        var updateResult = _unitOfWork.Sellers.Update(seller);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}
