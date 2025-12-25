using System;
using CSharpFunctionalExtensions;
using Domain.Property;
using Domain.Property.VO;
using UseCases.Booking.Commands.CreateBooking;
using UseCases.Interfaces.Commands;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property
{
    public record CreatePropertyCommand( 
        AddressData AddressData, 
        PropertyDetailsData PropertyDetailsData,
        OwnershipData OwnershipData
        ) : ICommand<Guid>;
}