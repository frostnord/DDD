using AutoMapper;
using Presenter.DTOs.BookingDTO;
using UseCases.Booking.Commands;
using UseCasesBookingDto = UseCases.UseCases.DTO.Booking.BookingDto;

namespace Presenter.Mappings
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<CreateBookingRequest, CreateBookingCommand>();

            CreateMap<UseCasesBookingDto, BookingDto>();
        }
    }
}
