using System;

namespace Presenter.DTOs.PropertyDTO;

public sealed record OwnershipDto(
    Guid OwnerClientId,
    DateTime StartDate
);