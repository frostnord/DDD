using CSharpFunctionalExtensions;


namespace DDD.Domain.ValueObjects.ClientVO
{
    /// <summary>
    /// Объект значения, представляющий идентификатор клиента
    /// </summary>
    public class ClientId : TypedId<ClientId>
    {
        private ClientId(Guid value) : base(value) { }

        public static Result<ClientId> Create(Guid value)
            => TypedId<ClientId>.Create(value, v => new ClientId(v));

        public static ClientId New() => new(Guid.NewGuid());

    }
}