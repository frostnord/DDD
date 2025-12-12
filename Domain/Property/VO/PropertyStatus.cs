using Domain.Utilities;

namespace Domain.Property.VO
{
    /// <summary>
    /// Smart Enum статуса недвижимости
    /// </summary>
    public sealed class PropertyStatus : Enumeration<PropertyStatus>
    {
        public const int MAX_STATUS_LENGTH = 20;

        // Коды для хранения в БД
        public static readonly PropertyStatus ForSale = new(1, "ForSale", "в продаже");
        public static readonly PropertyStatus Reserved = new(2, "Reserved", "забронирован");
        public static readonly PropertyStatus Sold = new(3, "Sold", "продан");

        private readonly string _displayName;

        private PropertyStatus(int value, string name, string displayName) : base(value, name)
        {
            _displayName = displayName;
        }

        public string GetDisplayName() => _displayName;
    }
}