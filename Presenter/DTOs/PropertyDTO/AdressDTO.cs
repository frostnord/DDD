namespace Presenter.DTOs.PropertyDTO;

public sealed record AddressDTO(
    string Street,
    string City,
    int HomeNumber,
    int ZipCode,
    string Country
);