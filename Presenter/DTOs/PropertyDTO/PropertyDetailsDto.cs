namespace Presenter.DTOs.PropertyDTO;

public sealed record PropertyDetailsDto(
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