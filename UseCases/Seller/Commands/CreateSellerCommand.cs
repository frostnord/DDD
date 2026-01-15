using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Seller.Commands;

public record CreateSellerCommand(Guid ClientId) : ICommand<Guid>;