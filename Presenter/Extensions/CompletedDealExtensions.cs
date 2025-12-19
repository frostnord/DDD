using Domain.Deal;
using Domain.ValueObjects;
using Presenter.DTOs.CompletedDealDTO;

namespace Presenter.Extensions;



public static class CompletedDealExtensions
{

    public static CompletedDealDto ToDto(this CompletedDealEntity entity)
    {
        return new CompletedDealDto
        {
            Id = entity.Id.Value,
            BuyerClientId = entity.BuyerClientId.Value,
            SellerClientId = entity.SellerClientId.Value,
            PropertyId = entity.PropertyId.Value,
            DealDate = entity.DealDate,
            DealAmount = entity.DealAmount.Value,
            DealType = entity.DealType.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
