using Domain.Domain.Customers.Buyer;
using Presenter.DTOs;

namespace Presenter.Extensions
{
    public static class BuyerExtensions
    {
        public static BuyerDto ToDTO(this Buyer buyer)
        {
            if (buyer == null)
                return null;

            return new BuyerDto
            {
                Id = buyer.Id.Value,
                ClientId = buyer.ClientId.Value,
                RegisteredAt = DateTime.UtcNow // В реальной реализации это должно быть свойство в сущности
            };
        }
    }
}