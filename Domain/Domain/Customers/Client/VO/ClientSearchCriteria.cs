using CSharpFunctionalExtensions;
using Domain.Domain.Property.VO;
using Domain.Domain.ValueObjects;

namespace Domain.Domain.Customers.Client.VO
{
    /// <summary>
    /// Объект значения, представляющий критерии поиска клиента
    /// </summary>
    public class ClientSearchCriteria : ValueObject
    {
        
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
        public SmartPropertyType PreferredType { get; }
        
        
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
        /// <param name="preferredNumberOfRooms">Предпочтительное количество комнат</param>
        /// <param name="preferredFloor">Предпочтительный этаж</param>
        /// <param name="preferredTotalFloors">Предпочтительное общее количество этажей в здании</param>
        /// <param name="preferredType">Предпочтительный тип недвижимости</param>
        /// <param name="preferParking">Наличие предпочтения по парковке</param>
        /// <param name="preferredHeatingType">Предпочтительный тип отопления</param>
        /// <param name="preferredCondition">Предпочтительное состояние недвижимости</param>
        private ClientSearchCriteria( NumberOfRooms preferredNumberOfRooms, Floor preferredFloor, TotalFloors preferredTotalFloors,
            SmartPropertyType preferredType, bool? preferParking,
            HeatingType preferredHeatingType, PropertyCondition preferredCondition)
        {
            
            PreferredNumberOfRooms = preferredNumberOfRooms;
            PreferredFloor = preferredFloor;
            PreferredTotalFloors = preferredTotalFloors;
            PreferredType = preferredType;
            PreferParking = preferParking;
            PreferredHeatingType = preferredHeatingType;
            PreferredCondition = preferredCondition;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра критериев поиска клиента с возвратом результата
        /// </summary>
        /// <param name="preferredNumberOfRooms">Предпочтительное количество комнат</param>
        /// <param name="preferredFloor">Предпочтительный этаж</param>
        /// <returns>Result с экземпляром ClientSearchCriteria при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<ClientSearchCriteria> Create( NumberOfRooms preferredNumberOfRooms, Floor preferredFloor, TotalFloors preferredTotalFloors,
            SmartPropertyType preferredType, bool? preferParking, HeatingType preferredHeatingType, PropertyCondition preferredCondition)
        {
            // Дополнительная доменная валидация отношений между полями
            if (preferredFloor.Value > preferredTotalFloors.Value)
                return Result.Failure<ClientSearchCriteria>("Предпочтительный этаж не может быть больше общего количества этажей");

            return Result.Success(new ClientSearchCriteria( preferredNumberOfRooms, preferredFloor, preferredTotalFloors,
                preferredType, preferParking, preferredHeatingType, preferredCondition));
        }

        public bool Equals(ClientSearchCriteria other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                   Equals(PreferredNumberOfRooms, other.PreferredNumberOfRooms) &&
                   Equals(PreferredFloor, other.PreferredFloor) &&
                   Equals(PreferredTotalFloors, other.PreferredTotalFloors) &&
                   Nullable.Equals(PreferredType, other.PreferredType) &&
                   Nullable.Equals(PreferParking, other.PreferParking) &&
                   Equals(PreferredHeatingType, other.PreferredHeatingType) &&
                   Equals(PreferredCondition, other.PreferredCondition);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PreferredNumberOfRooms;
            yield return PreferredFloor;
            yield return PreferredTotalFloors;
            yield return PreferredType;
            yield return PreferParking;
            yield return PreferredHeatingType;
            yield return PreferredCondition;
        }

        public override int GetHashCode()
        {
            // HashCode.Combine в .NET 6+ поддерживает до 8 аргументов, поэтому разбиваем на части
            return HashCode.Combine(
                HashCode.Combine( PreferredNumberOfRooms, PreferredFloor, PreferredTotalFloors),
                HashCode.Combine(PreferredType, PreferParking, PreferredHeatingType),
                PreferredCondition?.GetHashCode() ?? 0);
        }

        public override string ToString()
        {
            var parts = new System.Collections.Generic.List<string>();
            
            parts.Add($"Rooms: {PreferredNumberOfRooms}");
            parts.Add($"Floor: {PreferredFloor}");
            if (PreferredTotalFloors != null) parts.Add($"TotalFloors: {PreferredTotalFloors}");
            parts.Add($"Type: {PreferredType.DisplayName}");
            if (PreferParking.HasValue) parts.Add($"Parking: {PreferParking}");
            if (PreferredHeatingType != null) parts.Add($"Heating: {PreferredHeatingType}");
            if (PreferredCondition != null) parts.Add($"Condition: {PreferredCondition}");

            return parts.Count > 0 ? string.Join(", ", parts) : "No criteria";
        }
    }
}