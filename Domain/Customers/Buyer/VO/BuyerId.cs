using System;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;

namespace Domain.Customers.Buyer.VO
{
    /// <summary>
    /// Объект значения, представляющий идентификатор покупателя
    /// </summary>
    public class BuyerId : TypedId<BuyerId>
    {
        private BuyerId(Guid value) : base(value)
        {
        }

        public static Result<BuyerId> Create(Guid value)
            => Create(value, v => new BuyerId(v));
    }
}