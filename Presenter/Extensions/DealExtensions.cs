using Domain.Deal;
using Presenter.DTOs;
using Presenter.DTOs.DealDTO;

namespace Presenter.Extensions
{
    public static class DealExtensions
    {
        public static DealResponse ToDTO(this DealEntity deal)
        {
            return new DealResponse
            (
                deal.Id.Value,
                deal.ClientId.Value,
                deal.PropertyId.Value,
                deal.BookingId?.Value,
                deal.Details,
                deal.Status.Name,
                deal.CreatedAt,
                deal.UpdatedAt
            );
        }
    }
}