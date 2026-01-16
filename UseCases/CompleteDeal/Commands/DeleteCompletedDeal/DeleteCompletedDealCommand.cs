using System;
using UseCases.Interfaces.Commands;

namespace UseCases.CompleteDeal.Commands.DeleteCompletedDeal;

public sealed record DeleteCompletedDealCommand(Guid CompletedDealId) : ICommand;
