using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal.Commands;

public record ConfirmDealCommand(Guid DealId) : ICommand;
