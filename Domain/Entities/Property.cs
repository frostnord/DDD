using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;
using static DDD.Domain.Entities.OwnershipHistory;

namespace DDD.Domain.Entities
{
    /// <summary>
    /// Сущность объекта недвижимости в системе управления недвижимостью
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Уникальный идентификатор объекта недвижимости
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Адрес объекта недвижимости
        /// </summary>
        public Address Address { get; private set; }
        
        /// <summary>
        /// Цена объекта недвижимости
        /// </summary>
        public Price Price { get; private set; }
        
        /// <summary>
        /// Статус объекта недвижимости
        /// </summary>
        public PropertyStatus Status { get; private set; }
        
        /// <summary>
        /// История владения объектом недвижимости
        /// </summary>
        public OwnershipHistory OwnershipHistory { get; private set; }
        
        /// <summary>
        /// Описание объекта недвижимости
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// Тип недвижимости (квартира, дом, коммерческое помещение)
        /// </summary>
        public string Type { get; private set; }
        
        /// <summary>
        /// Площадь в квадратных метрах
        /// </summary>
        public int Area { get; private set; }
        
        /// <summary>
        /// Доступность объекта недвижимости
        /// </summary>
        public bool IsAvailable { get; private set; }
        
        /// <summary>
        /// Дата создания записи об объекте недвижимости
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления записи об объекте недвижимости
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр объекта недвижимости
        /// </summary>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="type">Тип недвижимости</param>
        /// <param name="area">Площадь в квадратных метрах</param>
        /// <param name="status">Статус недвижимости</param>
        private Property(Address address, Price price, string description, string type, int area, PropertyStatus status)
        {
            Id = Guid.NewGuid();
            Address = address;
            Price = price; //
            Description = description; //
            Type = type; // Тип
            Area = area; //площадь помещения
            IsAvailable = true;
            CreatedAt = DateTime.UtcNow;
            Status = status;
            OwnershipHistory = new OwnershipHistory();
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра объекта недвижимости с возвратом результата
        /// </summary>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="type">Тип недвижимости</param>
        /// <param name="area">Площадь в квадратных метрах</param>
        /// <param name="ownerName">Имя владельца</param>
        /// <param name="startDate">Дата начала владения</param>
        /// <param name="ownershipReason">Причина владения</param>
        /// <param name="status">Статус недвижимости</param>
        /// <returns>Result с экземпляром Property при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Property> Create(Address address, Price price, string description, string type, int area, string ownerName, DateTime startDate, string ownershipReason)
        {
            var validationErrors = new List<string>();

            // Проверка строковых значений на пустоту
            if (string.IsNullOrWhiteSpace(description))
                validationErrors.Add("Описание не может быть пустым");

            if (string.IsNullOrWhiteSpace(type))
                validationErrors.Add("Тип недвижимости не может быть пустым");

            // Проверка числовых значений на корректность
            if (area <= 0)
                validationErrors.Add("Площадь должна быть положительной");

            // Создание записи о владельце с валидацией
            var ownerRecordResult = OwnershipRecord.Create(ownerName, startDate, ownershipReason);
            if (ownerRecordResult.IsFailure)
            {
                validationErrors.Add(ownerRecordResult.Error);
            }

            // Возврат результата валидации
            return validationErrors.Count > 0
                ? Result.Failure<Property>(string.Join("; ", validationErrors))
                : Result.Success(CreateWithOwner(address, price, description, type, area, ownerRecordResult.Value));
        }

        /// <summary>
        /// Внутренний метод создания Property с владельцем
        /// </summary>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="type">Тип недвижимости</param>
        /// <param name="area">Площадь в квадратных метрах</param>
        /// <param name="ownerRecord">Запись о владельце</param>
        /// <returns>Экземпляр Property</returns>
        private static Property CreateWithOwner(Address address, Price price, string description, string type, int area, OwnershipRecord ownerRecord)
        {
            var property = new Property(address, price, description, type, area, PropertyStatus.ForSale);
            property.OwnershipHistory.AddRecord(ownerRecord);
            return property;
        }


        /// <summary>
        /// Добавляет запись в историю владения
        /// </summary>
        /// <param name="record">Запись истории владения</param>
        /// <exception cref="ArgumentNullException">Вызывается, если запись пуста</exception>
        public void AddOwnershipRecord(OwnershipRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record), "Запись истории владения не может быть пустой");
            }

            OwnershipHistory.AddRecord(record);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Получает текущего владельца недвижимости
        /// </summary>
        /// <returns>Запись о текущем владельце или null, если нет владельцев</returns>
        public OwnershipRecord GetCurrentOwner()
        {
            return OwnershipHistory.GetCurrentOwner();
        }

        /// <summary>
        /// Обновляет цену объекта недвижимости
        /// </summary>
        /// <param name="newPrice">Новая цена</param>
        /// <exception cref="ArgumentNullException">Вызывается, если новая цена пуста</exception>
        public void UpdatePrice(Price newPrice)
        {
            if (newPrice == null)
            {
                throw new ArgumentNullException(nameof(newPrice), "Цена не может быть пустой");
            }
            
            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет доступность объекта недвижимости
        /// </summary>
        /// <param name="isAvailable">Доступность объекта</param>
        public void UpdateAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет описание объекта недвижимости
        /// </summary>
        /// <param name="newDescription">Новое описание</param>
        /// <exception cref="ArgumentException">Вызывается, если новое описание пусто</exception>
        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                throw new ArgumentException("Описание не может быть пустым", nameof(newDescription));
            }
            
            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }
        public override string ToString()
        {
            return $"Недвижимость [ID: {Id}, Адрес: {Address}, Цена: {Price}, Статус: {Status.GetDisplayName()}, Тип: {Type}, Площадь: {Area} м², Доступна: {IsAvailable}]";
        }
    }

    
}