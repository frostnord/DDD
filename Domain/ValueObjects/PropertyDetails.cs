using System;

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
        public int Area { get; private set; }
        
        /// <summary>
        /// Количество комнат
        /// </summary>
        public int NumberOfRooms { get; private set; }
        
        /// <summary>
        /// Этаж
        /// </summary>
        public int Floor { get; private set; }
        
        /// <summary>
        /// Всего этажей в здании
        /// </summary>
        public int TotalFloors { get; private set; }
        
        /// <summary>
        /// Тип недвижимости: квартира, дом, коммерческое помещение
        /// </summary>
        public string Type { get; private set; }
        
        /// <summary>
        /// Наличие балкона
        /// </summary>
        public bool HasBalcony { get; private set; }
        
        /// <summary>
        /// Наличие парковки
        /// </summary>
        public bool HasParking { get; private set; }
        
        /// <summary>
        /// Тип отопления
        /// </summary>
        public string HeatingType { get; private set; }
        
        /// <summary>
        /// Состояние (новый, под ремонт, евроремонт и т.д.)
        /// </summary>
        public string Condition { get; private set; }

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
        /// <exception cref="ArgumentException">Вызывается, если данные некорректны</exception>
        public PropertyDetails(int area, int numberOfRooms, int floor, int totalFloors,
            string type, bool hasBalcony = false, bool hasParking = false,
            string heatingType = "Не указано", string condition = "Не указано")
        {
            if (area <= 0)
            {
                throw new ArgumentException("Площадь должна быть положительной", nameof(area));
            }
            
            if (numberOfRooms < 0)
            {
                throw new ArgumentException("Количество комнат не может быть отрицательным", nameof(numberOfRooms));
            }
            
            if (floor <= 0)
            {
                throw new ArgumentException("Этаж должен быть положительным", nameof(floor));
            }
            
            if (totalFloors <= 0)
            {
                throw new ArgumentException("Общее количество этажей должно быть положительным", nameof(totalFloors));
            }
            
            if (floor > totalFloors)
            {
                throw new ArgumentException("Номер этажа не может быть больше общего количества этажей", nameof(floor));
            }
            
            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Тип недвижимости не может быть пустым", nameof(type));
            }
            
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
        /// Возвращает площадь одной комнаты
        /// </summary>
        /// <returns>Площадь одной комнаты или 0, если невозможно рассчитать</returns>
        public int GetRoomArea() => Area > NumberOfRooms && NumberOfRooms > 0 ? Area / NumberOfRooms : 0;
    }
}