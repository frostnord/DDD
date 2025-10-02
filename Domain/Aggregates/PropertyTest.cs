using System;
using DDD.Domain.ValueObjects;
using Domain.ValueObjects;

namespace Domain.Aggregates
{
    /// <summary>
    /// Агрегат объекта недвижимости 
    /// </summary>
    public class PropertyTest
    {
        /// <summary>Уникальный идентификатор объекта недвижимости </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Адрес объекта недвижимости
        /// </summary>
        public Address Address { get; private set; }
        
        /// <summary>
        /// Статус объекта недвижимости
        /// </summary>
        public PropertyStatus Status { get; private set; }
        
        /// <summary>
        /// История владения объектом недвижимости
        /// </summary>
        public OwnershipHistory OwnershipHistory { get; private set; }
        
        /// <summary>
        /// Детали объекта недвижимости
        /// </summary>
        public PropertyDetails PropertyDetails { get; private set; }
        
        /// <summary>
        /// Цена объекта недвижимости
        /// </summary>
        public Price Price { get; private set; }
        
        /// <summary>
        /// Описание объекта недвижимости
        /// </summary>
        public string Description { get; private set; }
        
        /// <summary>
        /// Дата создания записи об объекте недвижимости
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления записи об объекте недвижимости
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }
        
        /// <param name="address">Адрес объекта недвижимости</param>
        /// <param name="status">Статус объекта недвижимости</param>
        /// <param name="ownershipHistory">История владения объектом недвижимости</param>
        /// <param name="propertyDetails">Детали объекта недвижимости</param>
        /// <param name="price">Цена объекта недвижимости</param>
        /// <param name="description">Описание объекта недвижимости</param>
        public PropertyTest(Address address, PropertyStatus status, OwnershipHistory ownershipHistory,
            PropertyDetails propertyDetails, Price price, string description = "")
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address), "Адрес не может быть пустым");
            }
            
            if (status == null)
            {
                throw new ArgumentNullException(nameof(status), "Статус недвижимости не может быть пустым");
            }
            
            if (ownershipHistory == null)
            {
                throw new ArgumentNullException(nameof(ownershipHistory), "История владения не может быть пустой");
            }
            
            if (propertyDetails == null)
            {
                throw new ArgumentNullException(nameof(propertyDetails), "Детали недвижимости не могут быть пустыми");
            }
            
            if (price == null)
            {
                throw new ArgumentNullException(nameof(price), "Цена не может быть пустой");
            }
            
            if (string.IsNullOrEmpty(description))
            {
                description = "Описание отсутствует";
            }
            
            Id = Guid.NewGuid();
            Address = address;
            Status = status;
            OwnershipHistory = ownershipHistory;
            PropertyDetails = propertyDetails;
            Price = price;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет статус объекта недвижимости
        /// </summary>
        /// <param name="newStatus">Новый статус</param>
        /// <exception cref="ArgumentNullException">Вызывается, если новый статус пуст</exception>
        public void UpdateStatus(PropertyStatus newStatus)
        {
            if (newStatus == null)
            {
                throw new ArgumentNullException(nameof(newStatus), "Статус недвижимости не может быть пустым");
            }
            
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
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
        /// Обновляет описание объекта недвижимости
        /// </summary>
        /// <param name="newDescription">Новое описание</param>
        /// <exception cref="ArgumentException">Вызывается, если новое описание пусто</exception>
        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrEmpty(newDescription))
            {
                throw new ArgumentException("Описание не может быть пустым", nameof(newDescription));
            }
            
            Description = newDescription;
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
            
            OwnershipHistory.AddRecord(record);
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Обновляет детали объекта недвижимости
        /// </summary>
        /// <param name="newDetails">Новые детали</param>
        /// <exception cref="ArgumentNullException">Вызывается, если новые детали пусты</exception>
        public void UpdatePropertyDetails(PropertyDetails newDetails)
        {
            if (newDetails == null)
            {
                throw new ArgumentNullException(nameof(newDetails), "Детали недвижимости не могут быть пустыми");
            }
            
            PropertyDetails = newDetails;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}