using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Buyer.Commands.DeleteBuyer
{
    public class DeleteBuyerCommandHandler : ICommandHandler<DeleteBuyerCommand>
    {
        private readonly IBuyerRepository _buyerRepository;

        public DeleteBuyerCommandHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        public async Task<Result> HandleAsync(DeleteBuyerCommand command)
        {
            var buyerId = BuyerId.Create(command.BuyerId);
            if (buyerId.IsFailure)
                return Result.Failure(buyerId.Error);

            var buyerResult = await _buyerRepository.GetByIdAsync(buyerId.Value);
            if (buyerResult.IsFailure)
            {
                return Result.Failure($"Buyer with ID {command.BuyerId} does not exist");
            }

            return await _buyerRepository.DeleteAsync(buyerId.Value);
        }
    }
}