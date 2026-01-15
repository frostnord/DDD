using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking;

namespace UseCases.Interfaces.Services;

public interface IBookingService
{
    Task<Result<BookingEntity>> CreateBookingAsync(Guid propertyId, Guid clientId, DateTime bookingDate,
        DateTime startTime, DateTime endTime);

    Task<Result<BookingEntity>> GetBookingByIdAsync(Guid bookingId);
    Task<Result<IEnumerable<BookingEntity>>> GetAllBookingsAsync();
    Task<Result<IEnumerable<BookingEntity>>> GetByClientIdAsync(Guid clientId);
    Task<Result<IEnumerable<BookingEntity>>> GetByPropertyIdAsync(Guid propertyId);

    Task<Result> UpdateBookingAsync(Guid bookingId, Guid propertyId, Guid clientId, DateTime bookingDate,
        DateTime startTime, DateTime endTime);

    Task<Result> DeleteBookingAsync(Guid bookingId);
}