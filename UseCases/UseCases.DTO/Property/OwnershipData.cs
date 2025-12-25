using System;

namespace UseCases.UseCases.DTO.Property;

public record OwnershipData(
    Guid OwnerClientId,
    DateTime StartDate
    );