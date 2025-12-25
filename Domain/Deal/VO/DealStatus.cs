using Domain.Utilities;

namespace Domain.Deal
{
    public sealed class DealStatus : Enumeration<DealStatus>
    {
        public static readonly DealStatus Created = new DealStatus(0, "Created", "Сделка создана", true);
        public static readonly DealStatus Confirmed = new DealStatus(1, "Confirmed", "Сделка подтверждена", true);
        public static readonly DealStatus Completed = new DealStatus(2, "Completed", "Сделка завершена", false);
        public static readonly DealStatus Cancelled = new DealStatus(3, "Cancelled", "Сделка отменена", false);

        public string Description { get; } //test
        public bool IsActiveStatus { get; } //test

        private DealStatus(int value, string name, string description, bool isActive) : base(value, name)
        {
            Description = description;
            IsActiveStatus = isActive;
        }

        public bool IsActive() => IsActiveStatus;

        public bool CanTransitionTo(DealStatus newStatus)
        {
            // Логика перехода между статусами
            return (this == Created && (newStatus == Confirmed || newStatus == Cancelled)) ||
                   (this == Confirmed && (newStatus == Completed || newStatus == Cancelled)) ||
                   (this == Completed || this == Cancelled); // Завершенные и отмененные статусы - финальные
        }

        public override string ToString() => $"{Name} ({Description})";
    }
}