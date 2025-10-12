using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;

namespace DDD.Domain
{
    /// <summary>
    /// Сущность объекта недвижимости в системе управления недвижимостью
    /// </summary>
    public class Property : CSharpFunctionalExtensions.Entity<PropertyId>
    {
        private readonly List<OwnershipRecord> _ownershipHistory;
        
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
        /// История владения объектом недвижимости (только для чтения)
        /// </summary>
        public IReadOnlyList<OwnershipRecord> OwnershipHistory => _ownershipHistory.AsReadOnly();
        
        /// <summary>
        /// Описание объекта недвижимости
        /// </summary>
        public Description Description { get; private set; }
        
        
        /// <summary>
        /// Детали объекта недвижимости (площадь, комнаты, этаж и т.д.)
        /// </summary>
        public PropertyDetails Details { get; private set; }
        
        
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
        /// <param name="id"></param>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="details">Детали объекта недвижимости</param>
        /// <param name="status">Статус недвижимости</param>
        private Property(PropertyId id, Address address, Price price, Description description, PropertyDetails details, PropertyStatus status) : base(id)
        {
            Address = address;
            Price = price;
            Description = description;
            Details = details;
            CreatedAt = DateTime.UtcNow;
            Status = status;
            _ownershipHistory = new List<OwnershipRecord>();
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра объекта недвижимости с возвратом результата (правильный подход DDD)
        /// </summary>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="details">Детали объекта недвижимости</param>
        /// <param name="ownerRecord">Запись о первом владельце</param>
        /// <returns>Result с экземпляром Property при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Property> Create(
            Address address, 
            Price price, 
            Description description, 
            PropertyDetails details,
            OwnershipRecord ownerRecord)
        {
            var validationErrors = new List<string>();

            // Валидация входных параметров
            if (address == null)
                validationErrors.Add("Адрес не может быть пустым");
            
            if (price == null)
                validationErrors.Add("Цена не может быть пустой");
            
            if (description == null)
                validationErrors.Add("Описание не может быть пустым");
            
            if (details == null)
                validationErrors.Add("Детали недвижимости не могут быть пустыми");
            
            if (ownerRecord == null)
                validationErrors.Add("Запись о владельце не может быть пустой");
            
            var id = PropertyId.New();
            
            // AddEvent(new PropertyCreatedEvent(Id));

            // Возврат результата валидации
            return validationErrors.Count > 0
                ? Result.Failure<Property>(string.Join("; ", validationErrors))
                : Result.Success(CreateWithOwner( id ,address, price, description, details, ownerRecord));
        }

        /// <summary>
        /// Внутренний метод создания Property с владельцем
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="details">Детали объекта недвижимости</param>
        /// <param name="ownerRecord">Запись о владельце</param>
        /// <returns>Экземпляр Property</returns>
        private static Property CreateWithOwner(PropertyId id, Address address, Price price, Description description, PropertyDetails details, OwnershipRecord ownerRecord)
        {
            var property = new Property(id, address, price, description, details, PropertyStatus.ForSale);
            property.AddOwnershipRecord(ownerRecord);
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

            _ownershipHistory.Add(record);
            // Сортировка записей по дате начала владения
            _ownershipHistory.Sort((r1, r2) => r1.StartDate.CompareTo(r2.StartDate));
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Получает текущего владельца недвижимости
        /// </summary>
        /// <returns>Запись о текущем владельце или null, если нет владельцев</returns>
        public OwnershipRecord GetCurrentOwner()
        {
            if (!_ownershipHistory.Any())
            {
                return null;
            }
            
            // Возвращаем запись с самой поздней датой начала владения (текущий владелец)
            return _ownershipHistory.OrderByDescending(r => r.StartDate).FirstOrDefault();
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

        /// <summary>
        /// Обновляет описание объекта недвижимости
        /// </summary>
        /// <param name="newDescription">Новое описание</param>
        /// <exception cref="ArgumentException">Вызывается, если новое описание пусто</exception>
        public void UpdateDescription(Description newDescription)
        {
            if (newDescription == null)
            {
                throw new ArgumentNullException(nameof(newDescription), "Описание не может быть пустым");
            }
            
            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }
        public override string ToString()
        {
            return $"Недвижимость [ID: {Id}, Адрес: {Address}, Цена: {Price}, Статус: {Status.GetDisplayName()}, Площадь: {Details.Area}, Комнат: {Details.NumberOfRooms}, Этаж: {Details.Floor}/{Details.TotalFloors}]";
        }
        
        // public void ChangePrice(decimal newPrice)
        // {
        //     if (Status == PropertyStatus.Sold)
        //         throw new InvalidPropertyStateException("Нельзя изменить цену проданного объекта.");
        //
        //     var oldPrice = CurrentPrice.Amount;
        //     CurrentPrice = CurrentPrice.Change(newPrice);
        //     _priceHistory.Add(CurrentPrice);
        //
        //     AddEvent(new PropertyPriceChangedEvent(Id, oldPrice, newPrice));
        // }
        //
        // public void Reserve()
        // {
        //     if (Status != PropertyStatus.Available)
        //         throw new InvalidPropertyStateException("Зарезервировать можно только доступный объект.");
        //     ChangeStatus(PropertyStatus.Reserved);
        // }
        //
        // public void MarkAsSold()
        // {
        //     if (Status != PropertyStatus.Reserved)
        //         throw new InvalidPropertyStateException("Продать можно только забронированный объект.");
        //     ChangeStatus(PropertyStatus.Sold);
        // }
        //
        // private void ChangeStatus(PropertyStatus newStatus)
        // {
        //     Status = newStatus;
        //     AddEvent(new PropertyStatusChangedEvent(Id, newStatus));
        // }
        //
        // private void AddEvent(IDomainEvent @event) => _events.Add(@event);
        // public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();
    }

    
}