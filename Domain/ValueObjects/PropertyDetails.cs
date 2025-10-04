using System;
using CSharpFunctionalExtensions;
using DDD.Domain.ValueObjects;
using DDD.Domain.ValueObjects.PropertyDetailsVO;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий детали объекта недвижимости
    /// </summary>
    public class PropertyDetails
    {
        /// <summary>
        /// Площадь в квадратных метрах
        /// </summary>
        public Area Area { get; }
        
        /// <summary>
        /// Количество комнат
        /// </summary>
        public NumberOfRooms NumberOfRooms { get; }
        
        /// <summary>
        /// Этаж
        /// </summary>
        public Floor Floor { get; }
        
        /// <summary>
        /// Всего этажей в здании
        /// </summary>
        public TotalFloors TotalFloors { get; }
        
        /// <summary>
        /// Тип недвижимости: квартира, дом, коммерческое помещение
        /// </summary>
        public PropertyType Type { get; }
        
        /// <summary>
        /// Наличие балкона
        /// </summary>
        public bool HasBalcony { get; }
        
        /// <summary>
        /// Наличие парковки
        /// </summary>
        public bool HasParking { get; }
        
        /// <summary>
        /// Тип отопления
        /// </summary>
        public HeatingType HeatingType { get; }
        
        /// <summary>
        /// Состояние (новый, под ремонт, евроремонт и т.д.)
        /// </summary>
        public PropertyCondition Condition { get; }

        /// <summary>
        /// Создает новый экземпляр деталей объекта недвижимости
        /// </summary>
        /// <param name="area">Площадь в квадратных метрах</param>
        /// <param name="numberOfRooms">Количество комнат</param>
        /// <param name="floor">Этаж</param>
        /// <param name="totalFloors">Всего этажей в здании</param>
        /// <param name="type">Тип недвижимости</param>
        /// <param name="hasBalcony">Наличие балкона</param>
        /// <param name="hasParking">Наличие парковки</param>
        /// <param name="heatingType">Тип отопления</param>
        /// <param name="condition">Состояние</param>
        private PropertyDetails(Area area, NumberOfRooms numberOfRooms, Floor floor, TotalFloors totalFloors,
            PropertyType type, bool hasBalcony, bool hasParking,
            HeatingType heatingType, PropertyCondition condition)
        {
            Area = area;
            NumberOfRooms = numberOfRooms;
            Floor = floor;
            TotalFloors = totalFloors;
            Type = type;
            HasBalcony = hasBalcony;
            HasParking = hasParking;
            HeatingType = heatingType;
            Condition = condition;
        }

        /// <summary>
        /// Фабричный метод для создания PropertyDetails с возвратом Result
        /// </summary>
        public static Result<PropertyDetails> Create(int area, int numberOfRooms, int floor, int totalFloors,
            PropertyType type, bool hasBalcony = false, bool hasParking = false,
            string heatingType = null, string condition = null)
        {
            var errors = new List<string>();

            // Валидация Area
            var areaResult = Area.Create(area);
            if (areaResult.IsFailure)
            {
                errors.Add(areaResult.Error);
            }

            // Валидация NumberOfRooms
            var numberOfRoomsResult = NumberOfRooms.Create(numberOfRooms);
            if (numberOfRoomsResult.IsFailure)
            {
                errors.Add(numberOfRoomsResult.Error);
            }

            // Валидация TotalFloors
            var totalFloorsResult = TotalFloors.Create(totalFloors);
            if (totalFloorsResult.IsFailure)
            {
                errors.Add(totalFloorsResult.Error);
            }

            // Валидация Floor (с учетом totalFloors)
            var floorResult = Floor.Create(floor, totalFloors);
            if (floorResult.IsFailure)
            {
                errors.Add(floorResult.Error);
            }

            // Валидация HeatingType
            var heatingTypeResult = HeatingType.Create(heatingType);
            if (heatingTypeResult.IsFailure)
            {
                errors.Add(heatingTypeResult.Error);
            }

            // Валидация PropertyCondition
            var conditionResult = PropertyCondition.Create(condition);
            if (conditionResult.IsFailure)
            {
                errors.Add(conditionResult.Error);
            }

            // Если есть ошибки, возвращаем Failure
            if (errors.Count > 0)
            {
                return Result.Failure<PropertyDetails>(string.Join("; ", errors));
            }

            // Создаем PropertyDetails со всеми валидированными Value Objects
            return Result.Success(new PropertyDetails(
                areaResult.Value,
                numberOfRoomsResult.Value,
                floorResult.Value,
                totalFloorsResult.Value,
                type,
                hasBalcony,
                hasParking,
                heatingTypeResult.Value,
                conditionResult.Value
            ));
        }

        /// <summary>
        /// Возвращает площадь одной комнаты
        /// </summary>
        /// <returns>Площадь одной комнаты или 0, если невозможно рассчитать</returns>
        public int GetRoomArea() => Area.Value > NumberOfRooms.Value && NumberOfRooms.Value > 0 ? Area.Value / NumberOfRooms.Value : 0;
    }
}