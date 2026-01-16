namespace UseCases.Interfaces.Queries;

public interface IQuery;

public interface IQuery<out TResponse> : IQuery;