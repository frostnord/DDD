using Domain.Utilities;

namespace Domain.Domain.Deal
{
    /// <summary>
    /// Smart Enum для типа сделки
    /// </summary>
    public sealed class DealType : Enumeration<DealType>
    {
        public const int MAX_DEAL_TYPE_LENGTH = 20;

        public static readonly DealType Purchase = new(1, "Purchase", "покупка");
        public static readonly DealType Rent     = new(2, "Rent",     "аренда");
        public static readonly DealType Lease    = new(3, "Lease",    "лизинг");
        public static readonly DealType Exchange = new(4, "Exchange", "обмен");

        private readonly string _displayName;

        private DealType(int value, string name, string displayName) : base(value, name)
        {
            _displayName = displayName;
        }

        public string GetDisplayName() => _displayName;
    }
}
