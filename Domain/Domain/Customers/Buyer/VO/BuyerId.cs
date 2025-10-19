using CSharpFunctionalExtensions;

namespace Domain.Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий идентификатор покупателя
    /// </summary>
    public class BuyerId : TypedId<BuyerId>
    {
        private BuyerId(Guid value) : base(value) { }

        public static Result<BuyerId> Create(Guid value)
            => TypedId<BuyerId>.Create(value, v => new BuyerId(v));
    }
}
