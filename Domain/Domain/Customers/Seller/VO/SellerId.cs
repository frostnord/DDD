using CSharpFunctionalExtensions;
using Domain.Domain.ValueObjects;

namespace Domain.Domain.Customers.Seller.VO
{
    /// <summary>
    /// Объект значения, представляющий идентификатор продавца
    /// </summary>
    public class SellerId : TypedId<SellerId>
    {
        private SellerId(Guid value) : base(value) { }

        public static Result<SellerId> Create(Guid value)
            => TypedId<SellerId>.Create(value, v => new SellerId(v));
    }
}
