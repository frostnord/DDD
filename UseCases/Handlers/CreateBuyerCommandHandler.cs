using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Buyer;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateBuyerCommandHandler : ICommandHandler<CreateBuyerCommand, Buyer>
    {
        private readonly IBuyerRepository _buyerRepository;
        private readonly IClientRepository _clientRepository;

        public CreateBuyerCommandHandler(
            IBuyerRepository buyerRepository,
            IClientRepository clientRepository)
        {
            _buyerRepository = buyerRepository;
            _clientRepository = clientRepository;
        }

        public async Task<Result<Buyer>> HandleAsync(CreateBuyerCommand command)
        {
            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(command.ClientId);
            if (clientResult.IsFailure)
            {
                return Result.Failure<Buyer>($"Client with ID {command.ClientId.Value} does not exist");
            }

            var buyerResult = Buyer.Create(
                command.ClientId,
                command.SearchCriteria
            );

            if (buyerResult.IsFailure)
            {
                return Result.Failure<Buyer>(buyerResult.Error);
            }

            var saveResult = await _buyerRepository.AddAsync(buyerResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<Buyer>(saveResult.Error);
            }

            return Result.Success(buyerResult.Value);
        }
    }
}