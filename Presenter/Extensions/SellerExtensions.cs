using System;
using Domain.Customers.Seller;
using UseCases.DTO.Seller;

namespace Presenter.Extensions
{
    public static class SellerExtensions
    {
        public static SellerDto ToDTO(this SellerEntity sellerEntity)
        {
            if (sellerEntity == null)
                throw new ArgumentNullException(nameof(sellerEntity));

            return new SellerDto
            (
                sellerEntity.Id.Value,
                sellerEntity.ClientId.Value,
                sellerEntity.RegisteredAt
            );
        }
    }
}