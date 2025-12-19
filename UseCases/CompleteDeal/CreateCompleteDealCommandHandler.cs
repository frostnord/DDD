using CSharpFunctionalExtensions;
using Domain.Deal;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.CompleteDeal
{
    public class CreateCompleteDealCommandHandler : ICommandHandler<CreateCompleteDealCommand, CompletedDealEntity>
    {
        private readonly ICompletedDealService _completedDealService;

        public CreateCompleteDealCommandHandler(ICompletedDealService completedDealService)
        {
            _completedDealService = completedDealService;
        }

        public async Task<Result<CompletedDealEntity>> HandleAsync(CreateCompleteDealCommand command)
        {
            return await _completedDealService.CreateAsync(
                command.BuyerClientId,
                command.SellerClientId,
                command.PropertyId,
                command.DealDate,
                command.DealAmount,
                command.DealType);
        }
    }
}
