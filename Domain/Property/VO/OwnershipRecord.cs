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
        /// Клиент (владелец) по ClientId
        /// </summary>
        public ClientId OwnerClientId { get; }

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
        /// Идентификатор недвижимости, которой владеет владелец
        /// </summary>
        public PropertyId PropertyId { get; }

        /// <summary>
        /// Создает новый экземпляр записи о владельце
        /// </summary>
        /// <param name="ownerClientId">Клиент (владелец) по ClientId</param>
        /// <param name="ownerName">Имя владельца</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="ownershipReason">Причина владения</param>
        /// <param name="propertyId">Идентификатор недвижимости</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        private OwnershipRecord(ClientId ownerClientId, Name ownerName, DateTime startDate, PropertyId propertyId, DateTime? endDate = null)
        {
            OwnerClientId = ownerClientId;
            OwnerName = ownerName;
            StartDate = startDate;
            PropertyId = propertyId;
            EndDate = endDate;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра записи о владельце с возвратом результата
        /// </summary>
        /// <param name="ownerClientId">Клиент (владелец) по ClientId</param>
        /// <param name="ownerName">Имя владельца (строка)</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="ownershipReason">Причина владения</param>
        /// <param name="propertyId">Идентификатор недвижимости</param>
        /// <param name="endDate">Дата окончания владения (необязательно)</param>
        /// <returns>Result с экземпляром OwnershipRecord при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<OwnershipRecord> Create(ClientId ownerClientId, string ownerName, DateTime startDate, PropertyId propertyId, DateTime? endDate = null)
        {
            var errors = new List<string>();

            if (ownerClientId == null)
            {
                errors.Add("Идентификатор клиента не может быть пустым");
            }
            // Создание и валидация Name
            var nameResult = Name.Create(ownerName);
            if (nameResult.IsFailure)
            {
                errors.Add(nameResult.Error);
            }

            if (startDate == default(DateTime))
                errors.Add("Дата начала владения не может быть пустой");

            if (propertyId == null)
                errors.Add("Идентификатор недвижимости не может быть пустым");

            if (endDate.HasValue && startDate > endDate.Value)
                errors.Add("Дата начала владения не может быть позже даты окончания владения");

            return errors.Count > 0
                ? Result.Failure<OwnershipRecord>(string.Join("; ", errors))
                : Result.Success(new OwnershipRecord(ownerClientId, nameResult.Value, startDate, propertyId, endDate));
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
            
            return $"{OwnerName} ({period})";
        }
        public override bool Equals(object obj)
        {
            if (obj is OwnershipRecord other)
            {
                return OwnerClientId.Equals(other.OwnerClientId)
                    && OwnerName.Equals(other.OwnerName)
                    && StartDate.Equals(other.StartDate)
                    && PropertyId.Equals(other.PropertyId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OwnerClientId, OwnerName, StartDate, PropertyId);
        }
    }
}