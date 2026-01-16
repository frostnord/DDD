using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Buyer.Commands.CreateBuyer;

public sealed record CreateBuyerCommand(
    Guid ClientId,
    int PreferredNumberOfRooms,
    int PreferredFloor,
    int PreferredTotalFloors,
    string PreferredType,
    bool? PreferParking,
    string PreferredHeatingType,
    string PreferredCondition
) : ICommand<Guid>;