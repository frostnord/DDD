using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal.Commands;

public record CancelDealCommand(Guid DealId) : ICommand;
