using System;
using UseCases.Interfaces.Commands;

namespace UseCases.Booking.Commands.ConfirmBooking;

public sealed record ConfirmBookingCommand(Guid PropertyId, Guid ClientId) : ICommand;