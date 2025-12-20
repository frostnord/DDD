using UseCases.Interfaces.Commands;

namespace UseCases.CompleteDeal;

public sealed record CompleteDealCommand(Guid DealId) : ICommand;