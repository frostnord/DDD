using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий номер лицензии
    /// </summary>
    public class LicenseNumber : ValueObject
    {
        public const int MIN_LENGTH = 5;
        public const int MAX_LENGTH = 15;

        /// <summary>
        /// Номер лицензии
        /// </summary>
        public string Value { get; }
        
        /// <summary>
        /// Создает новый экземпляр номера лицензии
        /// </summary>
        /// <param name="value">Номер лицензии</param>
        private LicenseNumber(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра номера лицензии с возвратом результата
        /// </summary>
        /// <param name="value">Номер лицензии</param>
        /// <returns>Result с экземпляром LicenseNumber при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<LicenseNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<LicenseNumber>("Номер лицензии не может быть пустым");
            }

            if (value.Length < MIN_LENGTH)
            {
                return Result.Failure<LicenseNumber>($"Номер лицензии должен содержать минимум {MIN_LENGTH} символов");
            }

            if (value.Length > MAX_LENGTH)
            {
                return Result.Failure<LicenseNumber>($"Номер лицензии не может превышать {MAX_LENGTH} символов");
            }

            return Result.Success(new LicenseNumber(value.Trim()));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }

        public override string ToString()
        {
            return Value;
        }
    }
}