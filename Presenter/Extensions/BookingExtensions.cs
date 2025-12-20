using Domain.Booking;
using Presenter.DTOs;
using Presenter.DTOs.BookingDTO;

namespace Presenter.Extensions
{
    public static class BookingExtensions
    {
        public static BookingDto ToDTO(this BookingEntity bookingEntity)
        {
            if (bookingEntity == null)
                return null;

            return new BookingDto
            {
                Id = bookingEntity.Id.Value,
                ClientId = bookingEntity.ClientId.Value,
                PropertyId = bookingEntity.PropertyId.Value,
                StartDate = bookingEntity.BookingPeriod.StartDate,
                EndDate = bookingEntity.BookingPeriod.EndDate,
                TotalPrice = bookingEntity.TotalPrice.Value,
                CreatedAt = bookingEntity.CreatedAt,
                UpdatedAt = bookingEntity.UpdatedAt
            };
        }
    }
}