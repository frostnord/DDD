using System;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

    /// <summary>
    /// Объект значения, представляющий номер телефона
    /// </summary>
    public class PhoneNumber : ValueObject
    {
        public const int MAX_TELEPHONE_NUMBER = 11;
        public string Value { get; set; }
        
        // Регулярное выражление для проверки российских номеров телефонов
        // Поддерживаются форматы:
        // +7XXXXXXXXXX
        // 8XXXXXXXXXX
        // +7(XXX)XXXXXXX
        // 8(XXX)XXXXXXX
        // +7 XXX XXX XXXX
        // 8 XXX XXX XXXX
        // +7-XXX-XXX-XXXX
        // 8-XXX-XXX-XXXX
        private static readonly Regex PhoneRegex = new Regex(@"^(\+7|8)(\s|-|\()?([0-9]{3})(\s|-|\))?([0-9]{3})(\s|-)?([0-9]{2})(\s|-)?([0-9]{2})$");

        /// <summary>
        /// Создает новый экземпляр номера телефона
        /// </summary>
        /// <param name="value">Номер телефона</param>
        private PhoneNumber(string value)
        {
            Value = value;
        }
        
        /// <summary>
        /// Фабричный метод для создания экземпляра номера телефона с возвратом результата
        /// </summary>
        /// <param name="value">Номер телефона</param>
        /// <returns>Result с экземпляром PhoneNumber при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<PhoneNumber> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<PhoneNumber>("Номер телефона не может быть пустым");
            }
            
            // Проверяем, что номер телефона соответствует формату российского номера
            if (!IsValidRussianPhoneNumber(value))
            {
                return Result.Failure<PhoneNumber>("Некорректный формат российского номера телефона");
            }

            return Result.Success(new PhoneNumber(value));
        }

        /// <summary>
        /// Проверяет, что номер телефона соответствует формату российского номера
        /// </summary>
        /// <param name="phoneNumber">Номер телефона для проверки</param>
        /// <returns>True, если номер соответствует формату, иначе False</returns>
        private static bool IsValidRussianPhoneNumber(string phoneNumber)
        {
            return PhoneRegex.IsMatch(phoneNumber);
        }
        
        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    }
