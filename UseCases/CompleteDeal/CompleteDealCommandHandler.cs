using CSharpFunctionalExtensions;
using Domain.Deal;
using UseCases.Interfaces;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal
{
    public class CompleteDealCommand : ICommand
    {
        public Guid DealId { get; set; }
    }
    
    public class CompleteDealCommandHandler : ICommandHandler<CompleteDealCommand>
    {
        private readonly IDealService _dealService;

        public CompleteDealCommandHandler(IDealService dealService)
        {
            _dealService = dealService;
        }

        public async Task<Result> HandleAsync(CompleteDealCommand command)
        {
            var result = await _dealService.CompleteAsync(command.DealId);
            return result;
        }
    }
}