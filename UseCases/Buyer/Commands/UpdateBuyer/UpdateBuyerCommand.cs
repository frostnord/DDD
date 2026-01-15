using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Buyer.Commands.UpdateBuyer;

public sealed record UpdateBuyerCommand(
    Guid BuyerId,
    Guid ClientId,
    int PreferredNumberOfRooms,
    int PreferredFloor,
    int PreferredTotalFloors,
    string PreferredType,
    bool? PreferParking,
    string PreferredHeatingType,
    string PreferredCondition
) : ICommand;