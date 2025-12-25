using Domain.Booking;
using Presenter.DTOs;
using Presenter.DTOs.BookingDTO;

namespace Presenter.Extensions
{
    public static class BookingExtensions
    {
        public static BookingDto ToDTO(this BookingEntity bookingEntity)
        {
            return new BookingDto
            (
                bookingEntity.Id.Value,
                bookingEntity.ClientId.Value,
                bookingEntity.PropertyId.Value,
                bookingEntity.BookingPeriod.StartDate,
                bookingEntity.BookingPeriod.EndDate,
                bookingEntity.TotalPrice.Value,
                bookingEntity.CreatedAt,
                bookingEntity.UpdatedAt
            );
        }
    }
}