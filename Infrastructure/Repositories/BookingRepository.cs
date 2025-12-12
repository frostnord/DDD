using CSharpFunctionalExtensions;
using Domain.Domain.Booking;
using Domain.Domain.Booking.VO;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Property.VO;
using Microsoft.EntityFrameworkCore;
using UseCases.Interfaces.Repositories;

namespace Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Booking>> GetByIdAsync(BookingId id)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking != null
                ? Result.Success(booking)
                : Result.Failure<Booking>($"Booking with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<Booking>>> GetByClientIdAsync(ClientId clientId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.ClientId == clientId)
                .ToListAsync();

            return Result.Success<IEnumerable<Booking>>(bookings);
        }

        public async Task<Result<IEnumerable<Booking>>> GetByPropertyIdAsync(PropertyId propertyId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.PropertyId == propertyId)
                .ToListAsync();

            return Result.Success<IEnumerable<Booking>>(bookings);
        }

        public async Task<Result> SaveAsync(Booking booking)
        {
            if (booking == null)
                return Result.Failure("Booking cannot be null");

            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(BookingId id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
            {
                return Result.Failure($"Booking with ID {id.Value} not found");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<bool> ExistsAsync(BookingId id)
        {
            return await _context.Bookings.AnyAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings.ToListAsync();
        }
    }
}