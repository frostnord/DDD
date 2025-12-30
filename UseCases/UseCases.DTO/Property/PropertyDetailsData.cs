namespace UseCases.UseCases.DTO.Property;

public record PropertyDetailsData(
    decimal Price,
    string Description,
    int NumberOfRooms,
    int Floor,
    int TotalFloors,
    decimal Area,
    string Type,
    string HeatingType,
    string Condition,
    bool? HasParking
);