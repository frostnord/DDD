namespace UseCases.Clients.Commands
{
    public interface ICommand;

    public interface ICommand<out TResponse> : ICommand;
}