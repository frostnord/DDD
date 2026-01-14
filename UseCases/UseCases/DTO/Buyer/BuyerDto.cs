using System;

namespace UseCases.UseCases.DTO.Buyer
{
    public record BuyerDto(Guid Id, Guid ClientId, DateTime RegistrationDate);
}