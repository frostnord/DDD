using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.Booking;
using Domain.Domain.Booking.Booking;
using Domain.Domain.Booking.Booking.VO;
using Domain.Domain.Booking.VO;
using Domain.Domain.Customers.Client.VO;
using Domain.Domain.Property.Property.VO;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace UseCases.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс репозитория для работы с агрегатами Booking
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Получает бронирование по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Результат с бронированием или ошибкой, если бронирование не найдено</returns>
        Task<Result<Booking>> GetByIdAsync(BookingId id);

        /// <summary>
        /// Получает все бронирования клиента по его идентификатору
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список бронирований клиента</returns>
        Task<Result<IEnumerable<Booking>>> GetByClientIdAsync(ClientId clientId);

        /// <summary>
        /// Получает все бронирования объекта недвижимости по его идентификатору
        /// </summary>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <returns>Список бронирований объекта недвижимости</returns>
        Task<Result<IEnumerable<Booking>>> GetByPropertyIdAsync(PropertyId propertyId);

        /// <summary>
        /// Сохраняет бронирование в репозитории
        /// </summary>
        /// <param name="booking">Бронирование для сохранения</param>
        /// <returns>Результат операции</returns>
        Task<Result> SaveAsync(Booking booking);

        /// <summary>
        /// Удаляет бронирование из репозитория
        /// </summary>
        /// <param name="id">Идентификатор бронирования для удаления</param>
        /// <returns>Результат операции</returns>
        Task<Result> DeleteAsync(BookingId id);
        
        /// <summary>
        /// Проверяет, существует ли бронирование с указанным идентификатором
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>True, если бронирование существует, иначе False</returns>
        Task<bool> ExistsAsync(BookingId id);
    }
}