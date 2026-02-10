using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.Property.VO;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Property
{
    /// <summary>
    /// Сущность объекта недвижимости в системе управления недвижимостью
    /// </summary>
    public class PropertyEntity : Entity<PropertyId>
    {
        private OwnershipHistory _ownershipHistory;

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
        [NotMapped]
        public IReadOnlyList<OwnershipRecord> OwnershipHistory => _ownershipHistory.Records;

        /// <summary>
        /// Описание объекта недвижимости
        /// </summary>
        public Description Description { get; private set; }


        /// <summary>
        /// Детали объекта недвижимости (площадь, комнаты, этаж и т.д.)
        /// </summary>
        public PropertyDetails PropertyDetails { get; private set; }


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
        /// <param name="propertyDetails">Детали объекта недвижимости</param>
        /// <param name="status">Статус недвижимости</param>
        protected PropertyEntity(PropertyId id, Address address, Price price, Description description, PropertyDetails propertyDetails,
            PropertyStatus status) : base(id)
        {
            Address = address;
            Price = price;
            Description = description;
            PropertyDetails = propertyDetails;
            CreatedAt = DateTime.UtcNow;
            Status = status;
            _ownershipHistory = new OwnershipHistory();
        }
        
        // EF Core конструктор
        protected PropertyEntity()
        {
            _ownershipHistory = new OwnershipHistory();
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра объекта недвижимости с возвратом результата (правильный подход Domain)
        /// </summary>
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        /// <param name="details">Детали объекта недвижимости</param>
        /// <returns>Result с экземпляром Property при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<PropertyEntity> Create(
            Address address,
            Price price,
            Description description,
            PropertyDetails details)
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

            var id = PropertyId.Create(Guid.NewGuid()).Value;

            // AddEvent(new PropertyCreatedEvent(Id));

            // Возврат результата валидации
            if (validationErrors.Count > 0)
                return Result.Failure<PropertyEntity>(string.Join("; ", validationErrors));

            // Создание Property без владельца
            var status = PropertyStatus.FromName("ForSale");
            var property = new PropertyEntity(id, address, price, description, details, status);
            return Result.Success(property);
        }

        // Метод CreateWithOwner больше не используется, так как логика создания вынесена в PropertyFactory


        /// <summary>
        /// Устанавливает первого владельца недвижимости
        /// </summary>
        /// <param name="ownerRecord">Запись о владельце</param>
        /// <exception cref="ArgumentNullException">Вызывается, если запись пуста</exception>
        public void SetFirstOwner(OwnershipRecord ownerRecord)
        {
            if (ownerRecord == null)
            {
                throw new ArgumentNullException(nameof(ownerRecord), "Запись о владельце не может быть пустой");
            }

            _ownershipHistory.AddRecord(ownerRecord);
            UpdatedAt = DateTime.UtcNow;
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

            _ownershipHistory.AddRecord(record);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Получает текущего владельца недвижимости
        /// </summary>
        /// <returns>Запись о текущем владельце или null, если нет владельцев</returns>
        public OwnershipRecord GetCurrentOwner()
        {
            return _ownershipHistory.GetCurrentOwner();
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
            return
                $"Недвижимость [ID: {Id}, Адрес: {Address}, Цена: {Price}, Статус: {Status.GetDisplayName()}, Площадь: {PropertyDetails.Area}, Комнат: {PropertyDetails.NumberOfRooms}, Этаж: {PropertyDetails.Floor}/{PropertyDetails.TotalFloors}]";
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
        public void UpdateAddress(Address address)
        {
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(PropertyDetails details)
        {
            PropertyDetails = details;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateOwner(OwnershipRecord ownerRecord)
        {
            AddOwnershipRecord(ownerRecord);
        }
    }
}