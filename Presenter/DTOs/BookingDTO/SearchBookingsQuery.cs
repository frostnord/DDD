namespace Presenter.DTOs.BookingDTO
{
    public sealed record SearchBookingsQuery(
        Guid? ClientId,
        Guid? PropertyId
    );
}