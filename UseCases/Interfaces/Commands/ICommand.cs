namespace UseCases.Interfaces.Commands;

public interface ICommand;

public interface ICommand<out TResponse> : ICommand;