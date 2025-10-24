using CSharpFunctionalExtensions;
using Domain.Domain.ValueObjects;

namespace Domain.Domain.Customers.Buyer.VO
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