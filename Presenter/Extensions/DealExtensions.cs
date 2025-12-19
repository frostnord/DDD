using Domain.Deal;
using Presenter.DTOs;
using Presenter.DTOs.DealDTO;

namespace Presenter.Extensions
{
    public static class DealExtensions
    {
        public static DealDto ToDTO(this DealEntity deal)
        {
            return new DealDto
            {
                Id = deal.Id.Value,
                ClientId = deal.ClientId.Value,
                PropertyId = deal.PropertyId.Value,
                BookingId = deal.BookingId?.Value,
                Details = deal.Details,
                Status = deal.Status.Name,
                CreatedAt = deal.CreatedAt,
                UpdatedAt = deal.UpdatedAt
            };
        }
    }
}