using AutoMapper;
using Presenter.DTOs.DealDTO;
using UseCases.Deal.Commands;
using UseCasesDealDto = UseCases.UseCases.DTO.Deal.DealDto;

namespace Presenter.Mappings
{
    public class DealMappingProfile : Profile
    {
        public DealMappingProfile()
        {
            CreateMap<CreateDealRequest, CreateDealCommand>();
            CreateMap<UseCasesDealDto, DealResponse>();
        }
    }
}
