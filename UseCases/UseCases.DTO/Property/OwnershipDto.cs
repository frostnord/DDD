using System;

namespace UseCases.UseCases.DTO.Property;

public record OwnershipDto(
    Guid OwnerClientId,
    DateTime StartDate
    );