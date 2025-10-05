using System;

namespace DDD.Domain.ValueObjects
{
    /// <summary>
    /// Перечисление, представляющее тип недвижимости
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// Квартира
        /// </summary>
        Apartment,
        
        /// <summary>
        /// Дом
        /// </summary>
        House,
        
        /// <summary>
        /// Коммерческое помещение
        /// </summary>
        Commercial,
        
        /// <summary>
        /// Земельный участок
        /// </summary>
        Land,
        
        /// <summary>
        /// Таунхаус
        /// </summary>
        Townhouse,
        
        /// <summary>
        /// Студия
        /// </summary>
        Studio
    }

    /// <summary>
    /// Расширения для PropertyType
    /// </summary>
    public static class PropertyTypeExtensions
    {
        /// <summary>
        /// Получает человекочитаемое название типа недвижимости на русском языке
        /// </summary>
        /// <param name="type">Тип недвижимости</param>
        /// <returns>Строковое представление типа</returns>
        public static string GetDisplayName(this PropertyType type)
        {
            return type switch
            {
                PropertyType.Apartment => "Квартира",
                PropertyType.House => "Дом",
                PropertyType.Commercial => "Коммерческое помещение",
                PropertyType.Land => "Земельный участок",
                PropertyType.Townhouse => "Таунхаус",
                PropertyType.Studio => "Студия",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип недвижимости")
            };
        }
        

        /// <summary>
        /// Парсит строковое значение в PropertyType
        /// </summary>
        /// <param name="value">Строковое значение</param>
        /// <returns>PropertyType или null, если не удалось распарсить</returns>
        public static PropertyType? ParseFromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.ToLowerInvariant() switch
            {
                "квартира" or "apartment" => PropertyType.Apartment,
                "дом" or "house" => PropertyType.House,
                "коммерческое помещение" or "коммерческое" or "commercial" => PropertyType.Commercial,
                "земельный участок" or "участок" or "land" => PropertyType.Land,
                "таунхаус" or "townhouse" => PropertyType.Townhouse,
                "студия" or "studio" => PropertyType.Studio,
                _ => null
            };
        }
    }
}
