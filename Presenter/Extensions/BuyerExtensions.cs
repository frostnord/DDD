using Domain.Customers.Buyer;
using Presenter.DTOs;

namespace Presenter.Extensions
{
    public static class BuyerExtensions
    {
        public static BuyerDto ToDTO(this BuyerEntity buyerEntity)
        {
            if (buyerEntity == null)
                return null;

            return new BuyerDto
            {
                Id = buyerEntity.Id.Value,
                ClientId = buyerEntity.ClientId.Value,
                RegisteredAt = DateTime.UtcNow // В реальной реализации это должно быть свойство в сущности
            };
        }
    }
}