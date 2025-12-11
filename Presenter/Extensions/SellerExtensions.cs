using Domain.Domain.Customers.Seller;
using Presenter.DTOs;

namespace Presenter.Extensions
{
    public static class SellerExtensions
    {
        public static SellerDto ToDTO(this Seller seller)
        {
            if (seller == null)
                return null;

            return new SellerDto
            {
                Id = seller.Id.Value,
                ClientId = seller.ClientId.Value,
                RegisteredAt = DateTime.UtcNow 
            };
        }
    }
}