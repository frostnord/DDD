namespace UseCases.Property.Queries.GetPropertyById;

public sealed record PropertyDto (
    Guid Id,
    AddressDto AddressDto, 
    PropertyDetailsDto PropertyDetailsDto,
    OwnershipDto OwnershipDto
);

public sealed record AddressDto(
    string Street,
    string City,
    int HomeNumber,
    int ZipCode,
    string Country
);

public record PropertyDetailsDto(
    decimal Price,
    string Description,
    int NumberOfRooms,
    int Floor,
    int TotalFloors,
    decimal Area,
    string Type,  
    string Heating, 
    string Condition,
    bool? HasParking
);

public record OwnershipDto(
    Guid OwnerClientId,
    DateTime StartDate
);