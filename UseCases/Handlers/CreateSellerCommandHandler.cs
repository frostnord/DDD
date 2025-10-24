using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Seller;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateSellerCommandHandler : ICommandHandler<CreateSellerCommand, Seller>
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

        public async Task<Result<Seller>> HandleAsync(CreateSellerCommand command)
        {
            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(command.ClientId);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Seller>($"Client with ID {command.ClientId.Value} does not exist");
            }

            var sellerResult = Seller.Create(
                command.ClientId
            );

            if (sellerResult.IsFailure)
            {
                return Result.Failure<Seller>(sellerResult.Error);
            }

            var saveResult = await _sellerRepository.AddAsync(sellerResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Seller>(saveResult.Error);
            }

            return Result.Success(sellerResult.Value);
        }
    }
}