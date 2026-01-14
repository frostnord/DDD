namespace Presenter.DTOs.PropertyDTO.Response
{
    public sealed record PropertyResponse(
        Guid Id,
        AddressDto Address,
        PropertyDetailsDto PropertyDetails,
        OwnershipDto Ownership
    );
}
