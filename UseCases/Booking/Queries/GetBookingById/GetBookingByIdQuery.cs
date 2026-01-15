using System;
using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.Booking;

namespace UseCases.Booking.Queries.GetBookingById;

public sealed record GetBookingByIdQuery(Guid BookingId) : IQuery<Result<BookingDto>>;
