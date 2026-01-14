using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Seller.Commands
{
    public record UpdateSellerCommand(Guid SellerId, Guid ClientId) : ICommand;
}