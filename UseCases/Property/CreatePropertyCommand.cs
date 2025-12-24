using System;
using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Interfaces.Commands;

namespace UseCases.Property
{
    public record CreatePropertyCommand(
        string Street,
        string City,
        int HomeNumber,
        int ZipCode,
        string Country,
        decimal Price,
        string Description,
        int NumberOfRooms,
        int Floor,
        int TotalFloors,
        string PropertyType,
        string HeatingType,
        string PropertyCondition,
        decimal Area,
        bool? HasParking,
        Guid OwnerClientId,
        DateTime StartDate
    ) : ICommand<Guid>;
}