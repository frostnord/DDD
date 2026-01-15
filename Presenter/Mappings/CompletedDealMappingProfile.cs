using AutoMapper;
using Domain.Deal;
using Presenter.DTOs.CompletedDealDTO;
using UseCases.CompleteDeal;
using UseCasesCompletedDealDto = UseCases.UseCases.DTO.CompletedDeal.CompletedDealDto;

namespace Presenter.Mappings
{
    public class CompletedDealMappingProfile : Profile
    {
        public CompletedDealMappingProfile()
        {
            CreateMap<CreateCompletedDealRequest, CreateCompleteDealCommand>();
            CreateMap<UseCasesCompletedDealDto, CompletedDealDto>();
            CreateMap<CompletedDealEntity, CompletedDealDto>();
        }
    }
}
