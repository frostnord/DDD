namespace UseCases.UseCases.DTO.Property;

public sealed record PropertyDto (
    Guid Id,
    AddressDto AddressDto, 
    PropertyDetailsDto PropertyDetailsDto,
    OwnershipDto OwnershipDto
);