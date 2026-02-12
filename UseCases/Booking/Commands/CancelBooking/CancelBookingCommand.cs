using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.CancelBooking;

public sealed record CancelBookingCommand(Guid PropertyId, Guid ClientId) : ICommand;