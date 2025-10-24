using CSharpFunctionalExtensions;
using Domain.Domain.Deal;
using UseCases.Commands;
using UseCases.Interfaces.Repositories;

namespace UseCases.Handlers
{
    public class CreateCompleteDealCommandHandler : ICommandHandler<CreateCompleteDealCommand, CompletedDeal>
    {
        private readonly ICompletedDealRepository _completedDealRepository;

        public CreateCompleteDealCommandHandler(ICompletedDealRepository completedDealRepository)
        {
            _completedDealRepository = completedDealRepository;
        }

        public async Task<Result<CompletedDeal>> HandleAsync(CreateCompleteDealCommand command)
        {
            // Создаем завершенную сделку
            var completedDealResult = CompletedDeal.Create(
                command.BuyerClientId,
                command.SellerClientId,
                command.PropertyId,
                command.DealDate,
                command.DealAmount,
                command.DealType
            );

            if (completedDealResult.IsFailure)
            {
                return Result.Failure<CompletedDeal>(completedDealResult.Error);
            }

            // Добавляем завершенную сделку в репозиторий
            var addResult = await _completedDealRepository.AddAsync(completedDealResult.Value);
            if (addResult.IsFailure)
            {
                return Result.Failure<CompletedDeal>(addResult.Error);
            }

            return Result.Success(completedDealResult.Value);
        }
    }
}