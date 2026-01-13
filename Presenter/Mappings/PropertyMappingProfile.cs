using AutoMapper;
using Presenter.DTOs.PropertyDTO.CreatePoperty;
using Presenter.DTOs.PropertyDTO.UpdateProperty;
using UseCases.Property.Commands.CreateProperty;
using UseCases.Property.Commands.UpdateProperty;

namespace Presenter.Mappings
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            // Маппинг для создания объекта
            CreateMap<CreatePropertyRequest, CreatePropertyCommand>();

            // Маппинг для обновления объекта
            CreateMap<UpdatePropertyRequest, UpdatePropertyCommand>()
                .ForMember(dest => dest.PropertyId, opt =>
                    opt.MapFrom((src, dest, destMember, context) => context.Items["Id"]));
        }
    }
}