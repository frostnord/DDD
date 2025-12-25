using System;
using Domain.Customers.Buyer;
using Presenter.DTOs;
using Presenter.DTOs.BuyerDTO;

namespace Presenter.Extensions
{
    public static class BuyerExtensions
    {
        public static BuyerDto ToDTO(this BuyerEntity buyerEntity)
        {
            if (buyerEntity == null)
                return null;

            return new BuyerDto
            (
                buyerEntity.Id.Value,
                buyerEntity.ClientId.Value,
                DateTime.UtcNow // В реальной реализации это должно быть свойство в сущности
            );
        }
    }
}