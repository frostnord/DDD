using AutoMapper;
using Presenter.DTOs.BuyerDTO;
using UseCases.Buyer.Commands.CreateBuyer;
using UseCases.Buyer.Commands.UpdateBuyer;

namespace Presenter.Mappings
{
    public class BuyerMappingProfile : Profile
    {
        public BuyerMappingProfile()
        {
            CreateMap<CreateBuyerRequest, CreateBuyerCommand>();

            CreateMap<UpdateBuyerRequest, UpdateBuyerCommand>()
                .ForMember(dest => dest.BuyerId, opt =>
                    opt.MapFrom((src, dest, destMember, context) => context.Items["Id"]));
        }
    }
}
