namespace UseCases.UseCases.DTO.Property;

public record AddressData(
    string Street,
    string City,
    int HomeNumber,
    int ZipCode,
    string Country
    );