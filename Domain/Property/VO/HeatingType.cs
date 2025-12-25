using CSharpFunctionalExtensions;
using Domain.Utilities;

namespace Domain.Property.VO
{
    /// <summary>
    /// Smart Enum для типа отопления
    /// </summary>
    public sealed class HeatingType : Enumeration<HeatingType>
    {
        public const int MAX_HEATING_TYPE_LENGTH = 5;

        // Предопределённые значения (коды для хранения в БД)
        public static readonly HeatingType Unknown = new(0, "Unknown");
        public static readonly HeatingType Central = new(1, "Central");
        public static readonly HeatingType Gas = new(2, "Gas");
        public static readonly HeatingType Electric = new(3, "Electric");
        public static readonly HeatingType Autonomous = new(4, "Autonomous");
        public static readonly HeatingType Stove = new(5, "Stove");

        private HeatingType(int value, string name) : base(value, name)
        {
        }

        /// <summary>
        /// Создание по названию (для совместимости с текущим доменом)
        /// Пустое значение маппится в Unknown
        /// </summary>
        public static Result<HeatingType> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Success(Unknown);

            var trimmed = name.Trim();
            try
            {
                return Result.Success(FromName(trimmed));
            }
            catch
            {
                return Result.Failure<HeatingType>($"Недопустимый тип отопления: {trimmed}");
            }
        }

        public override string ToString() => Name;
    }
}