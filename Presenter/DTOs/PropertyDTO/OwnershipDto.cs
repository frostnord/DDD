using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO;

public record OwnershipDto(
    [Required] Guid OwnerClientId,
    DateTime StartDate
);
