using CSharpFunctionalExtensions;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий запись о владельце недвижимости
    /// </summary>
    public class OwnershipRecord
    {
        /// <summary>
        /// Имя владельца
        /// </summary>
        public string OwnerName { get; private set; }
        
        /// <summary>
        /// Дата начала владения
        /// </summary>
        public DateTime StartDate { get; private set; }
        
        /// <summary>
        /// Дата окончания владения
        /// </summary>
        public DateTime? EndDate { get; private set; }
        
        /// <summary>
        /// Причина владения (покупка, наследство и т.д.)
        /// </summary>
        public string OwnershipReason { get; private set; }

        /// <summary>
        /// Создает новый экземпляр записи о владельце
        /// </summary>
        /// <param name="ownerName">Имя владельца</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="ownershipReason">Причина владения</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        /// <exception cref="ArgumentException">Вызывается, если данные владельца некорректны</exception>
        private OwnershipRecord(string ownerName, DateTime startDate, string ownershipReason, DateTime? endDate = null)
        {
            OwnerName = ownerName;
            StartDate = startDate;
            OwnershipReason = ownershipReason;
            EndDate = endDate;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра записи о владельце с возвратом результата
        /// </summary>
        /// <param name="ownerName">Имя владельца</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="ownershipReason">Причина владения</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        /// <returns>Result с экземпляром OwnershipRecord при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<OwnershipRecord> Create(string ownerName, DateTime startDate, string ownershipReason, DateTime? endDate = null)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(ownerName))
                errors.Add("Имя владельца не может быть пустым");

            if (startDate == default(DateTime))
                errors.Add("Дата начала владения не может быть пустой");

            if (string.IsNullOrWhiteSpace(ownershipReason))
                errors.Add("Причина владения не может быть пустой");

            if (endDate.HasValue && startDate > endDate.Value)
                errors.Add("Дата начала владения не может быть позже даты окончания владения");

            return errors.Count > 0
                ? Result.Failure<OwnershipRecord>(string.Join("; ", errors))
                : Result.Success(new OwnershipRecord(ownerName, startDate, ownershipReason, endDate));
        }

        /// <summary>
        /// Устанавливает дату окончания владения
        /// </summary>
        /// <param name="endDate">Дата окончания владения</param>
        /// <exception cref="ArgumentException">Вызывается, если дата некорректна</exception>
        public void SetEndDate(DateTime endDate)
        {
            if (endDate < StartDate)
            {
                throw new ArgumentException("Дата окончания владения не может быть раньше даты начала владения", nameof(endDate));
            }
            
            EndDate = endDate;
        }

        /// <summary>
        /// Проверяет, является ли владелец текущим
        /// </summary>
        public bool IsCurrentOwner => !EndDate.HasValue;
    }
}