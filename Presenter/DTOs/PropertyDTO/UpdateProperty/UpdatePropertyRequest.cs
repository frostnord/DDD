using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO.UpdateProperty
{
    public class UpdatePropertyRequest
    {
        [Required] public AddressDto Address { get; init; }

        [Required] public PropertyDetailsDto PropertyDetails { get; init; }

        [Required] public OwnershipDto Ownership { get; init; }
    }
}