using System;

namespace UseCases.DTO.Buyer
{
    public record BuyerDto(Guid Id, Guid ClientId, DateTime RegisteredAt);
}