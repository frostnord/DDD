using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
using UseCases.Interfaces.Repositories;
using UseCases.Interfaces.Services;

namespace UseCases.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Result<BookingEntity>> GetBookingByIdAsync(Guid bookingId)
        {
            var bookingIdResult = BookingId.Create(bookingId);
            if (bookingIdResult.IsFailure)
            {
                return Result.Failure<BookingEntity>("Invalid booking ID");
            }

            return await _bookingRepository.GetByIdAsync(bookingIdResult.Value);
        }

        public async Task<Result<IEnumerable<BookingEntity>>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return Result.Success(bookings.Value);
        }

        public async Task<Result<IEnumerable<BookingEntity>>> GetByClientIdAsync(Guid clientId)
        {
            var clientIdResult = ClientId.Create(clientId);
            if (clientIdResult.IsFailure)
            {
                return Result.Failure<IEnumerable<BookingEntity>>("Invalid client ID");
            }

            return await _bookingRepository.GetByClientIdAsync(clientIdResult.Value);
        }

        public async Task<Result<IEnumerable<BookingEntity>>> GetByPropertyIdAsync(Guid propertyId)
        {
            var propertyIdResult = PropertyId.Create(propertyId);
            if (propertyIdResult.IsFailure)
            {
                return Result.Failure<IEnumerable<BookingEntity>>("Invalid property ID");
            }

            return await _bookingRepository.GetByPropertyIdAsync(propertyIdResult.Value);
        }

        public async Task<Result<BookingEntity>> CreateBookingAsync(Guid propertyId, Guid clientId, DateTime bookingDate, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException("This method should not be implemented in this service as per requirements");
        }

        public async Task<Result> UpdateBookingAsync(Guid bookingId, Guid propertyId, Guid clientId, DateTime bookingDate, DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException("This method should not be implemented in this service as per requirements");
        }

        public async Task<Result> DeleteBookingAsync(Guid bookingId)
        {
            throw new NotImplementedException("This method should not be implemented in this service as per requirements");
        }
    }
}
