using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Seller.Commands
{
    public record DeleteSellerCommand(Guid SellerId) : ICommand;
}