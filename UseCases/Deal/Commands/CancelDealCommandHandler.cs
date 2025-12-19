using CSharpFunctionalExtensions;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.Deal.Commands
{
    public class CancelDealCommand : ICommand
    {
        public Guid DealId { get; set; }
    }
    
    public class CancelDealCommandHandler : ICommandHandler<CancelDealCommand>
    {
        private readonly IDealService _dealService;

        public CancelDealCommandHandler(IDealService dealService)
        {
            _dealService = dealService;
        }

        public async Task<Result> HandleAsync(CancelDealCommand command)
        {
            var result = await _dealService.CancelAsync(command.DealId);
            return result;
        }
    }
}