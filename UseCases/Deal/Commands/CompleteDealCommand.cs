using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal.Commands;

public sealed record CompleteDealCommand(Guid DealId) : ICommand;