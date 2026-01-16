using System;
using Domain.Deal;
using UseCases.Interfaces.Commands;

namespace UseCases.Deal.Commands;

public record CreateDealCommand(
    Guid ClientId,
    Guid PropertyId,
    Guid? BookingId,
    DealDetails Details
) : ICommand<Guid>;