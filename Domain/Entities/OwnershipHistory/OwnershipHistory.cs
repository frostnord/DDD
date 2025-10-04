using Domain.ValueObjects;

namespace DDD.Domain.Entities
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

    
    
    
}