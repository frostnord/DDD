using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.Domain;
using Domain.Domain.ValueObjects;
using Domain.Repositories;
using Domain.ValueObjects;

namespace Domain.Services
{
    /// <summary>
    /// Сервис для работы с бронированиями
    /// </summary>
    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;

        /// <summary>
        /// Создает новый экземпляр сервиса бронирований
        /// </summary>
        /// <param name="bookingRepository">Репозиторий бронирований</param>
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        }

        /// <summary>
        /// Создает новое бронирование
        /// </summary>
        /// <param name="client">Клиент, создающий бронирование</param>
        /// <param name="property">Объект недвижимости для бронирования</param>
        /// <param name="agency">Агентство, осуществляющее бронирование</param>
        /// <param name="bookingPeriod">Период бронирования</param>
        /// <param name="totalPrice">Общая цена бронирования</param>
        /// <returns>Результат с бронированием или ошибкой</returns>
        public Result<Booking> CreateBooking(Client client, Property property, Agency agency, Period bookingPeriod, Price totalPrice)
        {
            try
            {
                // Доверяем агрегату выполнение валидации
                var bookingResult = Booking.Create(client, property, agency, bookingPeriod, totalPrice);
                if (bookingResult.IsFailure)
                    return Result.Failure<Booking>(bookingResult.Error);

                // Работаем с репозиторием
                var saveResult = _bookingRepository.Save(bookingResult.Value);

                return saveResult.IsSuccess
                    ? Result.Success(bookingResult.Value)
                    : Result.Failure<Booking>(saveResult.Error);
            }
            catch (Exception ex)
            {
                return Result.Failure<Booking>($"Ошибка при создании бронирования: {ex.Message}");
            }
        }

        /// <summary>
        /// Получает бронирование по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Результат с бронированием или ошибкой, если бронирование не найдено</returns>
        public Result<Booking> GetBookingById(BookingId id)
        {
            return _bookingRepository.GetById(id);
        }

        /// <summary>
        /// Получает все бронирования клиента
        /// </summary>
        /// <param name="client">Клиент, чьи бронирования нужно получить</param>
        /// <returns>Список бронирований клиента</returns>
        public IReadOnlyList<Booking> GetBookingsByClient(Client client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            return _bookingRepository.GetByClientId(client.Id);
        }

        /// <summary>
        /// Получает все бронирования объекта недвижимости
        /// </summary>
        /// <param name="property">Объект недвижимости, чьи бронирования нужно получить</param>
        /// <returns>Список бронирований объекта недвижимости</returns>
        public IReadOnlyList<Booking> GetBookingsByProperty(Property property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            return _bookingRepository.GetByPropertyId(property.Id);
        }

        /// <summary>
        /// Подтверждает бронирование
        /// </summary>
        /// <param name="bookingId">Идентификатор бронирования для подтверждения</param>
        /// <returns>Результат операции</returns>
        public Result ConfirmBooking(BookingId bookingId)
        {
            var bookingResult = _bookingRepository.GetById(bookingId);
            if (bookingResult.IsFailure)
                return Result.Failure($"Бронирование с ID {bookingId} не найдено");

            try
            {
                bookingResult.Value.Confirm();
                return _bookingRepository.Save(bookingResult.Value);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка при подтверждении бронирования: {ex.Message}");
            }
        }

        /// <summary>
        /// Отменяет бронирование
        /// </summary>
        /// <param name="bookingId">Идентификатор бронирования для отмены</param>
        /// <returns>Результат операции</returns>
        public Result CancelBooking(BookingId bookingId)
        {
            var bookingResult = _bookingRepository.GetById(bookingId);
            if (bookingResult.IsFailure)
                return Result.Failure($"Бронирование с ID {bookingId} не найдено");

            try
            {
                bookingResult.Value.Cancel();
                return _bookingRepository.Save(bookingResult.Value);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка при отмене бронирования: {ex.Message}");
            }
        }

        /// <summary>
        /// Завершает бронирование
        /// </summary>
        /// <param name="bookingId">Идентификатор бронирования для завершения</param>
        /// <returns>Результат операции</returns>
        public Result CompleteBooking(BookingId bookingId)
        {
            var bookingResult = _bookingRepository.GetById(bookingId);
            if (bookingResult.IsFailure)
                return Result.Failure($"Бронирование с ID {bookingId} не найдено");

            try
            {
                bookingResult.Value.Complete();
                return _bookingRepository.Save(bookingResult.Value);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Ошибка при завершении бронирования: {ex.Message}");
            }
        }
    }
}