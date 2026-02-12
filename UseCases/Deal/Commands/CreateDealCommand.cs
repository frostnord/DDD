using System;
using Domain.Deal;
using Domain.Deal.VO;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal.Commands;

public record CreateDealCommand(
    Guid ClientId,
    Guid PropertyId,
    DealDetails Details
) : ICommand<Guid>;