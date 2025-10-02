using System;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий период времени
    /// </summary>
    public class Period
    {
        /// <summary>
        /// Дата начала периода
        /// </summary>
        public DateTime StartDate { get; private set; }
        
        /// <summary>
        /// Дата окончания периода
        /// </summary>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Создает новый экземпляр периода
        /// </summary>
        /// <param name="startDate">Дата начала</param>
        /// <param name="endDate">Дата окончания</param>
        /// <exception cref="ArgumentException">Вызывается, если даты некорректны</exception>
        public Period(DateTime startDate, DateTime endDate)
        {
            if (startDate == default(DateTime))
            {
                throw new ArgumentException("Дата начала не может быть пустой", nameof(startDate));
            }
            
            if (endDate == default(DateTime))
            {
                throw new ArgumentException("Дата окончания не может быть пустой", nameof(endDate));
            }
            
            if (startDate > endDate)
            {
                throw new ArgumentException("Дата начала не может быть позже даты окончания", nameof(startDate));
            }
            
            // Проверка на слишком большой период (например, более 100 лет)
            if (endDate.Year - startDate.Year > 100)
            {
                throw new ArgumentException("Период не может быть слишком большим (более 100 лет)", nameof(endDate));
            }
            
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Проверяет, содержится ли указанная дата в периоде
        /// </summary>
        /// <param name="date">Дата для проверки</param>
        /// <returns>True, если дата содержится в периоде, иначе false</returns>
        public bool Contains(DateTime date)
        {
            return date >= StartDate && date <= EndDate;
        }

        /// <summary>
        /// Возвращает продолжительность периода
        /// </summary>
        public TimeSpan Duration => EndDate - StartDate;
    }
}