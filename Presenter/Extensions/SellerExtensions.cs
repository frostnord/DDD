using System;
using Domain.Customers.Seller;
using Presenter.DTOs;
using Presenter.DTOs.SellerDTO;

namespace Presenter.Extensions
{
    public static class SellerExtensions
    {
        public static SellerDto ToDTO(this SellerEntity sellerEntity)
        {
            if (sellerEntity == null)
                return null;

            return new SellerDto
            {
                Id = sellerEntity.Id.Value,
                ClientId = sellerEntity.ClientId.Value,
                RegisteredAt = DateTime.UtcNow
            };
        }
    }
}