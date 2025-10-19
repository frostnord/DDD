using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Domain.Utilities;

namespace Domain.Domain.ValueObjects
{
    /// <summary>
    /// "Умный" тип недвижимости, соответствующий принципам Domain
    /// </summary>
    public sealed class SmartPropertyType : Enumeration<SmartPropertyType>
    {
        public static readonly SmartPropertyType Apartment = new SmartPropertyType(1, "Apartment", "Квартира");
        public static readonly SmartPropertyType House = new SmartPropertyType(2, "House", "Дом");
        public static readonly SmartPropertyType Commercial = new SmartPropertyType(3, "Commercial", "Коммерческое помещение");
        public static readonly SmartPropertyType Land = new SmartPropertyType(4, "Land", "Земельный участок");
        public static readonly SmartPropertyType Townhouse = new SmartPropertyType(5, "Townhouse", "Таунхаус");
        public static readonly SmartPropertyType Studio = new SmartPropertyType(6, "Studio", "Студия");

        /// <summary>
        /// Название типа недвижимости
        /// </summary>
        public string DisplayName { get; }

        private SmartPropertyType(int value, string name, string displayName) : base(value, name)
        {
            DisplayName = displayName;
        }
        
        /// <summary>
        /// Проверяет, является ли тип недвижимости жилым
        /// </summary>
        /// <returns>true, если тип является жилым</returns>
        public bool IsResidential()
        {
            return this == Apartment || this == House || this == Townhouse || this == Studio;
        }

        /// <summary>
        /// Проверяет, является ли тип недвижимости коммерческим
        /// </summary>
        /// <returns>true, если тип является коммерческим</returns>
        public bool IsCommercial()
        {
            return this == Commercial;
        }

        /// <summary>
        /// Проверяет, является ли тип недвижимости земельным участком
        /// </summary>
        /// <returns>true, если тип является земельным участком</returns>
        public bool IsLand()
        {
            return this == Land;
        }

        public override string ToString()
        {
            return $"{Name} ({DisplayName})";
        }
    }
}