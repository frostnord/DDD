namespace UseCases.UseCases.DTO.Property;

public record AddressDto(
    string Street,
    string City,
    int HomeNumber,
    int ZipCode,
    string Country
    );