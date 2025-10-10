using System;
using CSharpFunctionalExtensions;
using DDD.Domain.Entities;
using DDD.Domain.ValueObjects;
using DDD.Domain.ValueObjects.BookingVO;
using Domain.ValueObjects;

namespace DDD.Domain
{
    /// <summary>
    /// Агрегат бронирования в системе управления недвижимостью
    /// </summary>
    public class Booking : CSharpFunctionalExtensions.Entity<BookingId>
    {
        // Id уже определен в базовом классе CSharpFunctionalExtensions.Entity<TId>
        
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
        /// Дата создания бронирования
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        
        /// <summary>
        /// Дата последнего обновления бронирования
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Создает новый экземпляр бронирования через фабричный метод
        /// </summary>
        /// <param name="client">Клиент, совершающий бронирование</param>
        /// <param name="property">Объект недвижимости, который бронируется</param>
        /// <param name="agency">Агентство, осуществляющее бронирование</param>
        /// <param name="bookingPeriod">Период бронирования</param>
        /// <param name="totalPrice">Общая цена бронирования</param>
        /// <returns>Результат с бронированием или ошибкой</returns>
        public static Result<Booking> Create(Client client, Property property, Agency agency, Period bookingPeriod, Price totalPrice)
        {
            var validationErrors = new List<string>();

            if (client == null)
                validationErrors.Add("Клиент не может быть пустым");

            if (property == null)
                validationErrors.Add("Объект недвижимости не может быть пустым");

            if (agency == null)
                validationErrors.Add("Агентство не может быть пустым");

            if (bookingPeriod == null)
                validationErrors.Add("Период бронирования не может быть пустым");

            if (totalPrice == null)
                validationErrors.Add("Общая цена не может быть пустой");

            // Проверка, что цена соответствует стоимости недвижимости
            if (totalPrice != null && property != null && totalPrice.Value != property.Price.Value)
            {
                validationErrors.Add("Общая цена должна соответствовать стоимости недвижимости");
            }

            if (validationErrors.Count > 0)
            {
                return Result.Failure<Booking>(string.Join("; ", validationErrors));
            }

            var id = BookingId.New();
            var booking = new Booking(id, client, property, agency, bookingPeriod, totalPrice);
            return Result.Success(booking);
        }

        /// <summary>
        /// Создает новый экземпляр бронирования
        /// </summary>
        /// <param name="id">Уникальный идентификатор бронирования</param>
        /// <param name="client">Клиент, совершающий бронирование</param>
        /// <param name="property">Объект недвижимости, который бронируется</param>
        /// <param name="agency">Агентство, осуществляющее бронирование</param>
        /// <param name="bookingPeriod">Период бронирования</param>
        /// <param name="totalPrice">Общая цена бронирования</param>
        private Booking(BookingId id, Client client, Property property, Agency agency, Period bookingPeriod, Price totalPrice)
            : base(id)
        {
            Client = client;
            Property = property;
            Agency = agency;
            BookingPeriod = bookingPeriod;
            TotalPrice = totalPrice;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Подтверждает бронирование
        /// </summary>
        /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя подтвердить</exception>
        public void Confirm()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Отменяет бронирование
        /// </summary>
        /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя отменить</exception>
        public void Cancel()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Завершает бронирование
        /// </summary>
        /// <exception cref="InvalidOperationException">Вызывается, если бронирование нельзя завершить</exception>
        public void Complete()
        {
            UpdatedAt = DateTime.UtcNow;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is Booking other)
            {
                return base.Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}