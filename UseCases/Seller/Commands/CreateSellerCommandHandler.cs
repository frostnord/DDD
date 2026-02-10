using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Seller.Commands;

public class CreateSellerCommandHandler : ICommandHandler<CreateSellerCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSellerCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> HandleAsync(CreateSellerCommand command)
    {
        var clientIdResult = ClientId.Create(command.ClientId);
        if (clientIdResult.IsFailure)
        {
            return Result.Failure<Guid>(clientIdResult.Error);
        }

        var clientResult = await _unitOfWork.Clients.GetByIdAsync(clientIdResult.Value);
        if (clientResult.IsFailure)
        {
            return Result.Failure<Guid>($"Client with ID {command.ClientId} does not exist");
        }

        var seller = SellerEntity.Create(clientIdResult.Value);
        if (seller.IsFailure)
        {
            return Result.Failure<Guid>(seller.Error);
        }

        var saveResult = _unitOfWork.Sellers.Add(seller.Value);
        if (saveResult.IsFailure)
        {
            return Result.Failure<Guid>(saveResult.Error);
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Success(seller.Value.Id.Value);
    }
}
