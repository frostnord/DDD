using Domain.Utilities;

namespace Domain.Booking.VO
{
    /// <summary>
    /// Smart Enum статуса hold-бронирования
    /// </summary>
    public sealed class BookingStatus : Enumeration<BookingStatus>
    {
        public const int MAX_STATUS_LENGTH = 20;

        public static readonly BookingStatus Active = new(1, "Active", "активно");
        public static readonly BookingStatus Expired = new(2, "Expired", "истекло");
        public static readonly BookingStatus Cancelled = new(3, "Cancelled", "отменено");
        public static readonly BookingStatus Completed = new(4, "Completed", "завершено");

        private readonly string _displayName;

        private BookingStatus(int value, string name, string displayName) : base(value, name)
        {
            _displayName = displayName;
        }

        public string GetDisplayName() => _displayName;
    }
}
