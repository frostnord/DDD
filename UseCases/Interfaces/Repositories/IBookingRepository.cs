using CSharpFunctionalExtensions;
using Domain.Booking;
using Domain.Booking.VO;
using Domain.Customers.Client.VO;
using Domain.Property.VO;

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
        Task<Result<BookingEntity>> GetByIdAsync(BookingId id);

        /// <summary>
        /// Получает все бронирования
        /// </summary>
        /// <returns>Список всех бронирований</returns>
        Task<IEnumerable<BookingEntity>> GetAllAsync();

        /// <summary>
        /// Получает все бронирования клиента по его идентификатору
        /// </summary>
        /// <param name="clientId">Идентификатор клиента</param>
        /// <returns>Список бронирований клиента</returns>
        Task<Result<IEnumerable<Domain.Booking.BookingEntity>>> GetByClientIdAsync(ClientId clientId);

        /// <summary>
        /// Получает все бронирования объекта недвижимости по его идентификатору
        /// </summary>
        /// <param name="propertyId">Идентификатор объекта недвижимости</param>
        /// <returns>Список бронирований объекта недвижимости</returns>
        Task<Result<IEnumerable<BookingEntity>>> GetByPropertyIdAsync(PropertyId propertyId);

        /// <summary>
        /// Сохраняет бронирование в репозитории
        /// </summary>
        /// <param name="bookingEntity">Бронирование для сохранения</param>
        /// <returns>Результат операции</returns>
        Task<Result> SaveAsync(Domain.Booking.BookingEntity bookingEntity);

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