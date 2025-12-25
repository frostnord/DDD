using System;

namespace UseCases.UseCases.DTO.Property;

public sealed record PropertyCreatedResult(
    Guid PropertyId,
    DateTime CreatedAt
);