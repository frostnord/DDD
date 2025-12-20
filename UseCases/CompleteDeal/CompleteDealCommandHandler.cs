using CSharpFunctionalExtensions;
using UseCases.Interfaces.Commands;
using UseCases.Interfaces.Services;

namespace UseCases.CompleteDeal
{
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