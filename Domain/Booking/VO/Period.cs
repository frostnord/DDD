using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий период времени
    /// </summary>
    public class Period : ValueObject
    {
        /// <summary>
        /// Дата начала периода
        /// </summary>
        public DateTime StartDate { get; }
        
        /// <summary>
        /// Дата окончания периода
        /// </summary>
        public DateTime EndDate { get; }

        /// <summary>
        /// Создает новый экземпляр периода
        /// </summary>
        /// <param name="startDate">Дата начала</param>
        /// <param name="endDate">Дата окончания</param>
        public Period(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра периода с возвратом результата
        /// </summary>
        /// <param name="startDate">Дата начала</param>
        /// <param name="endDate">Дата окончания</param>
        /// <returns>Result с экземпляром Period при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Period> Create(DateTime startDate, DateTime endDate)
        {
            var validationErrors = new List<string>();

            if (startDate == default(DateTime))
                validationErrors.Add("Дата начала не может быть пустой");

            if (endDate == default(DateTime))
                validationErrors.Add("Дата окончания не может быть пустой");

            if (startDate > endDate)
                validationErrors.Add("Дата начала не может быть позже даты окончания");

            // Проверка на слишком большой период (например, более 100 лет)
            if (endDate.Year - startDate.Year > 100)
                validationErrors.Add("Период не может быть слишком большим (более 100 лет)");

            return validationErrors.Count > 0
                ? Result.Failure<Period>(string.Join("; ", validationErrors))
                : Result.Success(new Period(startDate, endDate));
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }

        public override string ToString()
        {
            return $"Период с {StartDate:dd.MM.yyyy} по {EndDate:dd.MM.yyyy}";
        }
    }
}