using System;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;

namespace Domain.Customers.Client.VO
{
    /// <summary>
    /// Объект значения, представляющий идентификатор клиента
    /// </summary>
    public class ClientId : TypedId<ClientId>
    {
        private ClientId(Guid value) : base(value)
        {
        }

        public static Result<ClientId> Create(Guid value)
            => Create(value, v => new ClientId(v));
    }
}