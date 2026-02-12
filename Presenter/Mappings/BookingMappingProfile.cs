using AutoMapper;
using Presenter.DTOs.BookingDTO;
using UseCases.Reservation.Commands;
using UseCases.UseCases.DTO.Booking;

namespace Presenter.Mappings
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<CreateBookingRequest, CreateReservationCommand>();

            CreateMap<ReservationDto, BookingDto>();
        }
    }
}
