using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Customers.Buyer;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Buyer
{
    public class CreateBuyerCommandHandler : ICommandHandler<CreateBuyerCommand, BuyerEntity>
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

        public async Task<Result<BuyerEntity>> HandleAsync(CreateBuyerCommand command)
        {
            // Проверяем, существует ли клиент
            var clientResult = await _clientRepository.GetByIdAsync(command.ClientId);
            if (clientResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>($"Client with ID {command.ClientId.Value} does not exist");
            }

            var buyerResult = BuyerEntity.Create(
                command.ClientId,
                command.SearchCriteria
            );

            if (buyerResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(buyerResult.Error);
            }

            var saveResult = await _buyerRepository.AddAsync(buyerResult.Value);
            if (saveResult.IsFailure)
            {
                return Result.Failure<BuyerEntity>(saveResult.Error);
            }

            return Result.Success(buyerResult.Value);
        }
    }
}