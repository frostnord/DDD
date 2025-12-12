using CSharpFunctionalExtensions;
using Domain.Customers.Client.VO;
using Domain.Customers.Seller;
using UseCases.Clients.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateSellerCommandHandler : ICommandHandler<CreateSellerCommand, SellerEntity>
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IClientRepository _clientRepository;

        public CreateSellerCommandHandler(
            ISellerRepository sellerRepository,
            IClientRepository clientRepository)
        {
            _sellerRepository = sellerRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result<SellerEntity>> HandleAsync(CreateSellerCommand command)
        {
            var clientIdResult = ClientId.Create(command.ClientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<SellerEntity>(clientIdResult.Error);
            }

            var clientResult = await _clientRepository.GetByIdAsync(clientIdResult.Value);
            if (clientResult.IsFailure)
            {
                return Result.Failure<SellerEntity>($"Client with ID {command.ClientId} does not exist");
            }

            var sellerResult = SellerEntity.Create(clientIdResult.Value);
            if (sellerResult.IsFailure)
            {
                return Result.Failure<SellerEntity>(sellerResult.Error);
            }

            var saveResult = await _sellerRepository.AddAsync(sellerResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<SellerEntity>(saveResult.Error);
            }

            return Result.Success(sellerResult.Value);
        }
    }
}