using CSharpFunctionalExtensions;
using UseCases.Interfaces.Commands;

namespace UseCases.Property.Commands.DeleteProperty
{
    public record DeletePropertyCommand(
        Guid PropertyId
    ) : ICommand<Result>;
}