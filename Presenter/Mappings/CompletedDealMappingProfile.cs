using AutoMapper;
using Domain.Deal;
using Presenter.DTOs.CompletedDealDTO;
using UseCases.CompleteDeal;
using UseCases.CompleteDeal.Commands.CreateCompliteDealCommand;
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
