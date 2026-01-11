using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO.UpdateProperty
{
    public class UpdatePropertyRequest
    {
        [Required] public required AddressDto Address { get; init; }

        [Required] public required PropertyDetailsDto PropertyDetails { get; init; }

        [Required] public required OwnershipDto Ownership { get; init; }
    }
}