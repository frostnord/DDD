using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий историю владения недвижимостью
    /// </summary>
    public class OwnershipHistory
    {
        /// <summary>
        /// Список записей о владельцах
        /// </summary>
        public List<OwnershipRecord> Records { get; private set; }

        /// <summary>
        /// Создает новый экземпляр истории владения
        /// </summary>
        /// <param name="records">Список записей о владельцах</param>
        /// <exception cref="ArgumentNullException">Вызывается, если список записей пуст</exception>
        public OwnershipHistory(List<OwnershipRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records), "История владения не может быть пустой");
            }
            
            Records = new List<OwnershipRecord>(records);
        }

        /// <summary>
        /// Создает новый экземпляр истории владения с пустым списком
        /// </summary>
        public OwnershipHistory()
        {
            Records = new List<OwnershipRecord>();
        }

        /// <summary>
        /// Добавляет запись о владельце в историю
        /// </summary>
        /// <param name="record">Запись о владельце</param>
        /// <exception cref="ArgumentNullException">Вызывается, если запись пуста</exception>
        public void AddRecord(OwnershipRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record), "Запись истории владения не может быть пустой");
            }
            
            Records.Add(record);
            // Сортировка записей по дате начала владения
            Records = Records.OrderBy(r => r.StartDate).ToList();
        }

        /// <summary>
        /// Удаляет запись о владельце из истории
        /// </summary>
        /// <param name="record">Запись о владельце для удаления</param>
        /// <exception cref="ArgumentNullException">Вызывается, если запись пуста</exception>
        public void RemoveRecord(OwnershipRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record), "Запись истории владения не может быть пустой");
            }
            
            Records.Remove(record);
        }

        /// <summary>
        /// Возвращает текущего владельца недвижимости
        /// </summary>
        /// <returns>Запись о текущем владельце или null, если нет владельцев</returns>
        public OwnershipRecord GetCurrentOwner()
        {
            if (!Records.Any())
            {
                return null;
            }
            
            // Возвращаем запись самым поздним StartDate (текущий владелец)
            return Records.OrderByDescending(r => r.StartDate).FirstOrDefault();
        }
    }

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
        public OwnershipRecord(string ownerName, DateTime startDate, string ownershipReason, DateTime? endDate = null)
        {
            if (string.IsNullOrWhiteSpace(ownerName))
            {
                throw new ArgumentException("Имя владельца не может быть пустым", nameof(ownerName));
            }
            
            if (startDate == default(DateTime))
            {
                throw new ArgumentException("Дата начала владения не может быть пустой", nameof(startDate));
            }
            
            if (string.IsNullOrWhiteSpace(ownershipReason))
            {
                throw new ArgumentException("Причина владения не может быть пустой", nameof(ownershipReason));
            }
            
            if (endDate.HasValue && startDate > endDate.Value)
            {
                throw new ArgumentException("Дата начала владения не может быть позже даты окончания владения", nameof(startDate));
            }
            
            OwnerName = ownerName;
            StartDate = startDate;
            OwnershipReason = ownershipReason;
            EndDate = endDate;
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