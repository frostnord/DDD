using System;
using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;
using DDD.Domain.ValueObjects.PropertyDetailsVO;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий критерии поиска клиента
    /// </summary>
    public class ClientSearchCriteria : IEquatable<ClientSearchCriteria>
    {
        /// <summary>
        /// Предпочитаемая площадь недвижимости
        /// </summary>
        public Area PreferredArea { get; }
        
        /// <summary>
        /// Предпочтительное количество комнат
        /// </summary>
        public NumberOfRooms PreferredNumberOfRooms { get; }
        
        /// <summary>
        /// Предпочтительный этаж
        /// </summary>
        public Floor PreferredFloor { get; }
        
        /// <summary>
        /// Предпочтительное общее количество этажей в здании
        /// </summary>
        public TotalFloors PreferredTotalFloors { get; }
        
        /// <summary>
        /// Предпочтительный тип недвижимости
        /// </summary>
        public PropertyType? PreferredType { get; }
        
        /// <summary>
        /// Наличие предпочтения по балкону
        /// </summary>
        public bool? PreferBalcony { get; }
        
        /// <summary>
        /// Наличие предпочтения по парковке
        /// </summary>
        public bool? PreferParking { get; }
        
        /// <summary>
        /// Предпочтительный тип отопления
        /// </summary>
        public HeatingType PreferredHeatingType { get; }
        
        /// <summary>
        /// Предпочтительное состояние недвижимости
        /// </summary>
        public PropertyCondition PreferredCondition { get; }

        /// <summary>
        /// Создает новый экземпляр критериев поиска клиента
        /// </summary>
        /// <param name="preferredArea">Предпочитаемая площадь недвижимости</param>
        /// <param name="preferredNumberOfRooms">Предпочтительное количество комнат</param>
        /// <param name="preferredFloor">Предпочтительный этаж</param>
        /// <param name="preferredTotalFloors">Предпочтительное общее количество этажей в здании</param>
        /// <param name="preferredType">Предпочтительный тип недвижимости</param>
        /// <param name="preferBalcony">Наличие предпочтения по балкону</param>
        /// <param name="preferParking">Наличие предпочтения по парковке</param>
        /// <param name="preferredHeatingType">Предпочтительный тип отопления</param>
        /// <param name="preferredCondition">Предпочтительное состояние недвижимости</param>
        private ClientSearchCriteria(Area preferredArea, NumberOfRooms preferredNumberOfRooms, Floor preferredFloor, TotalFloors preferredTotalFloors,
            PropertyType? preferredType, bool? preferBalcony, bool? preferParking,
            HeatingType preferredHeatingType, PropertyCondition preferredCondition)
        {
            PreferredArea = preferredArea;
            PreferredNumberOfRooms = preferredNumberOfRooms;
            PreferredFloor = preferredFloor;
            PreferredTotalFloors = preferredTotalFloors;
            PreferredType = preferredType;
            PreferBalcony = preferBalcony;
            PreferParking = preferParking;
            PreferredHeatingType = preferredHeatingType;
            PreferredCondition = preferredCondition;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра критериев поиска клиента с возвратом результата
        /// </summary>
        /// <param name="preferredArea">Предпочитаемая площадь недвижимости</param>
        /// <param name="preferredNumberOfRooms">Предпочтительное количество комнат</param>
        /// <param name="preferredFloor">Предпочтительный этаж</param>
        /// <param name="preferredTotalFloors">Предпочтительное общее количество этажей в здании</param>
        /// <param name="preferredType">Предпочтительный тип недвижимости</param>
        /// <param name="preferBalcony">Наличие предпочтения по балкону</param>
        /// <param name="preferParking">Наличие предпочтения по парковке</param>
        /// <param name="preferredHeatingType">Предпочтительный тип отопления</param>
        /// <param name="preferredCondition">Предпочтительное состояние недвижимости</param>
        /// <returns>Result с экземпляром ClientSearchCriteria при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<ClientSearchCriteria> Create(Area preferredArea = null, NumberOfRooms preferredNumberOfRooms = null, Floor preferredFloor = null, TotalFloors preferredTotalFloors = null,
            PropertyType? preferredType = null, bool? preferBalcony = null, bool? preferParking = null,
            HeatingType preferredHeatingType = null, PropertyCondition preferredCondition = null)
        {
            // Валидация не требуется, так как все параметры опциональные
            // Возвращаем экземпляр с предоставленными значениями или null
            return Result.Success(new ClientSearchCriteria(preferredArea, preferredNumberOfRooms, preferredFloor, preferredTotalFloors,
                preferredType, preferBalcony, preferParking, preferredHeatingType, preferredCondition));
        }

        public override bool Equals(object obj)
        {
            if (obj is ClientSearchCriteria other)
            {
                return Equals(PreferredArea, other.PreferredArea) &&
                       Equals(PreferredNumberOfRooms, other.PreferredNumberOfRooms) &&
                       Equals(PreferredFloor, other.PreferredFloor) &&
                       Equals(PreferredTotalFloors, other.PreferredTotalFloors) &&
                       Nullable.Equals(PreferredType, other.PreferredType) &&
                       Nullable.Equals(PreferBalcony, other.PreferBalcony) &&
                       Nullable.Equals(PreferParking, other.PreferParking) &&
                       Equals(PreferredHeatingType, other.PreferredHeatingType) &&
                       Equals(PreferredCondition, other.PreferredCondition);
            }
            return false;
        }

        public bool Equals(ClientSearchCriteria other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(PreferredArea, other.PreferredArea) &&
                   Equals(PreferredNumberOfRooms, other.PreferredNumberOfRooms) &&
                   Equals(PreferredFloor, other.PreferredFloor) &&
                   Equals(PreferredTotalFloors, other.PreferredTotalFloors) &&
                   Nullable.Equals(PreferredType, other.PreferredType) &&
                   Nullable.Equals(PreferBalcony, other.PreferBalcony) &&
                   Nullable.Equals(PreferParking, other.PreferParking) &&
                   Equals(PreferredHeatingType, other.PreferredHeatingType) &&
                   Equals(PreferredCondition, other.PreferredCondition);
        }

        public override int GetHashCode()
        {
            // HashCode.Combine в .NET 6+ поддерживает до 8 аргументов, поэтому разбиваем на части
            return HashCode.Combine(
                HashCode.Combine(PreferredArea, PreferredNumberOfRooms, PreferredFloor, PreferredTotalFloors),
                HashCode.Combine(PreferredType, PreferBalcony, PreferParking, PreferredHeatingType),
                PreferredCondition?.GetHashCode() ?? 0);
        }

        public override string ToString()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (PreferredArea != null) parts.Add($"Area: {PreferredArea}");
            if (PreferredNumberOfRooms != null) parts.Add($"Rooms: {PreferredNumberOfRooms}");
            if (PreferredFloor != null) parts.Add($"Floor: {PreferredFloor}");
            if (PreferredTotalFloors != null) parts.Add($"TotalFloors: {PreferredTotalFloors}");
            if (PreferredType.HasValue) parts.Add($"Type: {PreferredType.Value.GetDisplayName()}");
            if (PreferBalcony.HasValue) parts.Add($"Balcony: {PreferBalcony.Value}");
            if (PreferParking.HasValue) parts.Add($"Parking: {PreferParking.Value}");
            if (PreferredHeatingType != null) parts.Add($"Heating: {PreferredHeatingType}");
            if (PreferredCondition != null) parts.Add($"Condition: {PreferredCondition}");

            return parts.Count > 0 ? string.Join(", ", parts) : "No criteria";
        }
    }
}