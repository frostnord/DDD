using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;
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

        public async Task<Result<BookingEntity>> GetByIdAsync(BookingId id)
        {
            var booking = await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking != null
                ? Result.Success(booking)
                : Result.Failure<BookingEntity>($"Booking with ID {id.Value} not found");
        }

        public async Task<Result<IEnumerable<BookingEntity>>> GetByClientIdAsync(ClientId clientId)
        {
            var bookings = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.ClientId == clientId)
                .ToListAsync();

            return Result.Success<IEnumerable<BookingEntity>>(bookings);
        }

        public async Task<Result<IEnumerable<BookingEntity>>> GetByPropertyIdAsync(PropertyId propertyId)
        {
            var bookings = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.PropertyId == propertyId)
                .ToListAsync();

            return Result.Success<IEnumerable<BookingEntity>>(bookings);
        }

        public Result Save(BookingEntity bookingEntity)
        {
            if (bookingEntity == null)
                return Result.Failure("Booking cannot be null");

            _context.Bookings.Add(bookingEntity);

            return Result.Success();
        }

        public Result Delete(BookingId id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                return Result.Failure($"Booking with ID {id.Value} not found");
            }

            _context.Bookings.Remove(booking);

            return Result.Success();
        }

        public async Task<bool> ExistsAsync(BookingId id)
        {
            return await _context.Bookings.AsNoTracking().AnyAsync(b => b.Id == id);
        }

        public async Task<Result<IEnumerable<BookingEntity>>> GetAllAsync()
        {
            return Result.Success<IEnumerable<BookingEntity>>(await _context.Bookings.AsNoTracking().ToListAsync());
        }
    }
}
