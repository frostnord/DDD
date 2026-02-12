using UseCases.UseCases.DTO.Booking;

namespace UseCases.Reservation.Queries.SearchReservationQuery;

public record SearchBookingsQueryResponse(List<ReservationDto> Items);
