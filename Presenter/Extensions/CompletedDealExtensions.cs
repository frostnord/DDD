using Domain.Deal;
using Domain.ValueObjects;
using Presenter.DTOs.CompletedDealDTO;

namespace Presenter.Extensions;



public static class CompletedDealExtensions
{

    public static CompletedDealDto ToDto(this CompletedDealEntity entity)
    {
        return new CompletedDealDto
        (
                entity.Id.Value,
                entity.BuyerClientId.Value,
                entity.SellerClientId.Value,
                entity.PropertyId.Value,
                entity.DealDate,
                entity.DealAmount.Value,
                entity.DealType.Name,
                entity.CreatedAt,
                entity.UpdatedAt
        );
    }
}
