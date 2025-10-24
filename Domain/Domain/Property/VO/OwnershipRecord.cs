using CSharpFunctionalExtensions;
using Domain.Domain.Customers.Client.VO;

namespace Domain.Domain.Property.VO
{
    /// <summary>
    /// Объект значения, представляющий запись о владельце недвижимости
    /// </summary>
    public class OwnershipRecord : ValueObject
    {
        /// <summary>
        /// Клиент (владелец) по ClientId
        /// </summary>
        public ClientId OwnerClientId { get; }
        
        /// <summary>
        /// Дата начала владения
        /// </summary>
        public DateTime StartDate { get; }
        /// <summary>
        /// Дата окончания владения
        /// </summary>
        public DateTime? EndDate { get; private set; }

        /// <summary>
        /// Создает новый экземпляр записи о владельце
        /// </summary>
        /// <param name="ownerClientId">Клиент (владелец) по ClientId</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        private OwnershipRecord(ClientId ownerClientId, DateTime startDate, DateTime? endDate = null)
        {
            OwnerClientId = ownerClientId;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра записи о владельце с возвратом результата
        /// </summary>
        /// <param name="ownerClientId">Клиент (владелец) по ClientId</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        /// <returns>Result с экземпляром OwnershipRecord при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<OwnershipRecord> Create(ClientId ownerClientId, DateTime startDate, DateTime? endDate = null)
        {
            var errors = new List<string>();

            if (ownerClientId == null)
            {
                errors.Add("Идентификатор клиента не может быть пустым");
            }

            if (startDate == default(DateTime))
                errors.Add("Дата начала владения не может быть пустой");

            if (endDate.HasValue && startDate > endDate.Value)
                errors.Add("Дата начала владения не может быть позже даты окончания владления");

            return errors.Count > 0
                ? Result.Failure<OwnershipRecord>(string.Join("; ", errors))
                : Result.Success(new OwnershipRecord(ownerClientId, startDate, endDate));
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
        
        public override string ToString()
        {
            var period = EndDate.HasValue 
                ? $"{StartDate:dd.MM.yyyy} - {EndDate.Value:dd.MM.yyyy}" 
                : $"с {StartDate:dd.MM.yyyy}";
            return $"Owner {OwnerClientId.Value} ({period})";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return OwnerClientId;
            yield return StartDate;
        }

        public override bool Equals(object obj)
        {
            if (obj is OwnershipRecord other)
            {
                return OwnerClientId.Equals(other.OwnerClientId)
                    && StartDate.Equals(other.StartDate);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OwnerClientId, StartDate);
        }
    }
}