using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using Domain.Customers.Seller.VO;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Seller.Commands
{
    public class UpdateSellerCommandHandler : ICommandHandler<UpdateSellerCommand>
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IClientRepository _clientRepository;

        public UpdateSellerCommandHandler(
            ISellerRepository sellerRepository,
            IClientRepository clientRepository)
        {
            _sellerRepository = sellerRepository;
            _clientRepository = clientRepository;
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

            var sellerResult = await _sellerRepository.GetByIdAsync(sellerIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure(sellerResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure($"Client with ID {command.ClientId} does not exist");
            }
        
            SellerEntity seller = sellerResult.Value;
            seller.Update(clientIdResult.Value);

            var updateResult = await _sellerRepository.UpdateAsync(seller);
            if (updateResult.IsFailure)
            {
                return Result.Failure(updateResult.Error);
            }

            return Result.Success();
        }
    }
}