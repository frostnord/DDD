using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;

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
        public Name OwnerName { get; }
        
        /// <summary>
        /// Дата начала владения
        /// </summary>
        public DateTime StartDate { get; }
        
        /// <summary>
        /// Дата окончания владения
        /// </summary>
        public DateTime? EndDate { get; private set; }
        
        /// <summary>
        /// Причина владения (покупка, наследство и т.д.)
        /// </summary>
        public string OwnershipReason { get; }

        /// <summary>
        /// Создает новый экземпляр записи о владельце
        /// </summary>
        /// <param name="ownerName">Имя владельца</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="ownershipReason">Причина владения</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        private OwnershipRecord(Name ownerName, DateTime startDate, string ownershipReason, DateTime? endDate = null)
        {
            OwnerName = ownerName;
            StartDate = startDate;
            OwnershipReason = ownershipReason;
            EndDate = endDate;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра записи о владельце с возвратом результата
        /// </summary>
        /// <param name="ownerName">Имя владельца (строка)</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="ownershipReason">Причина владения</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        /// <returns>Result с экземпляром OwnershipRecord при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<OwnershipRecord> Create(string ownerName, DateTime startDate, string ownershipReason, DateTime? endDate = null)
        {
            var errors = new List<string>();

            // Создание и валидация Name
            var nameResult = Name.Create(ownerName);
            if (nameResult.IsFailure)
            {
                errors.Add(nameResult.Error);
            }

            if (startDate == default(DateTime))
                errors.Add("Дата начала владения не может быть пустой");

            if (string.IsNullOrWhiteSpace(ownershipReason))
                errors.Add("Причина владения не может быть пустой");

            if (endDate.HasValue && startDate > endDate.Value)
                errors.Add("Дата начала владения не может быть позже даты окончания владения");

            return errors.Count > 0
                ? Result.Failure<OwnershipRecord>(string.Join("; ", errors))
                : Result.Success(new OwnershipRecord(nameResult.Value, startDate, ownershipReason, endDate));
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

        /// <summary>
        /// Получает инициалы владельца
        /// </summary>
        public string GetOwnerInitials() => OwnerName.GetInitials();

        /// <summary>
        /// Получает фамилию владельца
        /// </summary>
        public string GetOwnerLastName() => OwnerName.GetLastName();

        public override string ToString()
        {
            var period = EndDate.HasValue 
                ? $"{StartDate:dd.MM.yyyy} - {EndDate.Value:dd.MM.yyyy}" 
                : $"с {StartDate:dd.MM.yyyy}";
            
            return $"{OwnerName} ({OwnershipReason}, {period})";
        }

        public override bool Equals(object obj)
        {
            if (obj is OwnershipRecord other)
            {
                return OwnerName.Equals(other.OwnerName) 
                    && StartDate.Equals(other.StartDate)
                    && OwnershipReason.Equals(other.OwnershipReason);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OwnerName, StartDate, OwnershipReason);
        }
    }
}