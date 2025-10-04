using System;
using DDD.Domain.Entities;
using DDD.Domain.ValueObjects;
using Domain.Entities;
using Domain.ValueObjects;

namespace DDD.Domain.Aggregates
{
    /// <summary>
    /// Агрегат бронирования в системе управления недвижимостью
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Уникальный идентификатор бронирования
        /// </summary>
        public Guid Id { get; private set; }
        
        /// <summary>
        /// Клиент, совершающий бронирование
        /// </summary>
        public Client Client { get; private set; }
        
        /// <summary>
        /// Объект недвижимости, который бронируется
        /// </summary>
        public Property Property { get; private set; }
        
        /// <summary>
        /// Агентство, осуществляющее бронирование
        /// </summary>
        public Agency Agency { get; private set; }
        
        /// <summary>
        /// Период бронирования
        /// </summary>
        public Period BookingPeriod { get; private set; }
        
        /// <summary>
        /// Общая цена бронирования
        /// </summary>
        public Price TotalPrice { get; private set; }
        
        /// <summary>
        /// Статус бронирования (ожидание, подтверждено, отменено, завершено)
        /// </summary>
        public string Status { get; private set; }
        
        /// <summary>
        /// Дата создания бронирования
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления бронирования
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр бронирования
        /// </summary>
        /// <param name="client">Клиент, совершающий бронирование</param>
        /// <param name="property">Объект недвижимости, который бронируется</param>
        /// <param name="agency">Агентство, осуществляющее бронирование</param>
        /// <param name="bookingPeriod">Период бронирования</param>
        /// <param name="totalPrice">Общая цена бронирования</param>
        /// <exception cref="ArgumentNullException">Вызывается, если обязательные параметры пусты</exception>
        /// <exception cref="InvalidOperationException">Вызывается, если условия бронирования некорректны</exception>
        public Booking(Client client, Property property, Agency agency, Period bookingPeriod, Price totalPrice)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Клиент не может быть пустым");
            }
            
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property), "Объект недвижимости не может быть пустым");
            }
            
            if (agency == null)
            {
                throw new ArgumentNullException(nameof(agency), "Агентство не может быть пустым");
            }
            
            if (bookingPeriod == null)
            {
                throw new ArgumentNullException(nameof(bookingPeriod), "Период бронирования не может быть пустым");
            }
            
            if (totalPrice == null)
            {
                throw new ArgumentNullException(nameof(totalPrice), "Общая цена не может быть пустой");
            }
            
            // Проверка доступности недвижимости в указанный период
            if (!property.IsAvailable)
            {
                throw new InvalidOperationException("Объект недвижимости недоступен для бронирования");
            }
            
            // Проверка, что цена соответствует стоимости недвижимости
            if (totalPrice.Value != property.Price.Value)
            {
                throw new InvalidOperationException("Общая цена должна соответствовать стоимости недвижимости");
            }
            
            Id = Guid.NewGuid();
            Client = client;
            Property = property;
            Agency = agency;
            BookingPeriod = bookingPeriod;
            TotalPrice = totalPrice;
            Status = "pending"; // по умолчанию
            CreatedAt = DateTime.UtcNow;
            
            // Обновление статуса недвижимости
            Property.UpdateAvailability(false);
        }

        /// <summary>
        /// Подтверждает бронирование
        /// </summary>
        /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя подтвердить</exception>
        public void Confirm()
        {
            if (Status != "pending")
            {
                throw new InvalidOperationException($"Бронирование нельзя подтвердить, текущий статус: {Status}");
            }
            
            Status = "confirmed";
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Отменяет бронирование
        /// </summary>
        /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя отменить</exception>
        public void Cancel()
        {
            if (Status == "completed" || Status == "cancelled")
            {
                throw new InvalidOperationException($"Бронирование нельзя отменить, текущий статус: {Status}");
            }
            
            Status = "cancelled";
            UpdatedAt = DateTime.UtcNow;
            
            // Обновление статуса недвижимости
            Property.UpdateAvailability(true);
        }

        /// <summary>
        /// Завершает бронирование
        /// </summary>
        /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя завершить</exception>
        public void Complete()
        {
            if (Status != "confirmed")
            {
                throw new InvalidOperationException($"Бронирование нельзя завершить, текущий статус: {Status}");
            }
            
            Status = "completed";
            UpdatedAt = DateTime.UtcNow;
        }
        
    }
}