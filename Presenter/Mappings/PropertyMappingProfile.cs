using AutoMapper;
using Presenter.DTOs.PropertyDTO;
using Presenter.DTOs.PropertyDTO.Request.CreatePoperty;
using Presenter.DTOs.PropertyDTO.Request.UpdateProperty;
using Presenter.DTOs.PropertyDTO.Response;
using UseCases.Property.Commands;
using UseCases.Property.Commands.CreateProperty;
using UseCasesAddressDto = UseCases.UseCases.DTO.Property.AddressDto;
using UseCasesOwnershipDto = UseCases.UseCases.DTO.Property.OwnershipDto;
using UseCasesPropertyDetailsDto = UseCases.UseCases.DTO.Property.PropertyDetailsDto;
using UseCasesPropertyDto = UseCases.UseCases.DTO.Property.PropertyDto;

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

            CreateMap<UseCasesPropertyDto, PropertyResponse>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.AddressDto))
                .ForMember(dest => dest.PropertyDetails, opt => opt.MapFrom(src => src.PropertyDetailsDto))
                .ForMember(dest => dest.Ownership, opt => opt.MapFrom(src => src.OwnershipDto));

            CreateMap<UseCasesAddressDto, AddressDto>();
            CreateMap<UseCasesPropertyDetailsDto, PropertyDetailsDto>();
            CreateMap<UseCasesOwnershipDto, OwnershipDto>();
        }
    }
}