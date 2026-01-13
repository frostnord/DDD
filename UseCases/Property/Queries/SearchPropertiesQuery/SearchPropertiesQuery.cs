using CSharpFunctionalExtensions;
using UseCases.Interfaces.Queries;
using UseCases.UseCases.DTO.Property;

namespace UseCases.Property.Queries.SearchPropertiesQuery
{
    /// <summary>
    /// Запрос для поиска и фильтрации объектов недвижимости с пагинацией.
    /// </summary>
    public record SearchPropertiesQuery : IQuery<Result<SearchPropertiesQueryResponse>>
    {
        /// <summary>
        /// Город, в котором находится объект недвижимости.
        /// </summary>
        /// <example>Санкт-Петербург</example>
        public string? City { get; init; }

        /// <summary>
        /// Тип объекта.
        /// </summary>
        /// <example>Квартира</example>
        public string? PropertyType { get; init; }

        /// <summary>
        /// Минимальная цена.
        /// </summary>
        /// <example>50000</example>
        public decimal? MinPrice { get; init; }

        /// <summary>
        /// Максимальная цена.
        /// </summary>
        /// <example>150000</example>
        public decimal? MaxPrice { get; init; }

        /// <summary>
        /// Минимальная площадь в квадратных метрах.
        /// </summary>
        /// <example>40</example>
        public int? MinArea { get; init; }

        /// <summary>
        /// Максимальная площадь в квадратных метрах.
        /// </summary>
        /// <example>120</example>
        public int? MaxArea { get; init; }

        /// <summary>
        /// Минимальное количество комнат.
        /// </summary>
        /// <example>2</example>
        public int? MinRooms { get; init; }

        /// <summary>
        /// Максимальное количество комнат.
        /// </summary>
        /// <example>4</example>
        public int? MaxRooms { get; init; }

        /// <summary>
        /// Минимальный этаж.
        /// </summary>
        /// <example>3</example>
        public int? MinFloor { get; init; }

        /// <summary>
        /// Максимальный этаж.
        /// </summary>
        /// <example>9</example>
        public int? MaxFloor { get; init; }

        /// <summary>
        /// Тип отопления.
        /// </summary>
        /// <example>Центральное</example>
        public string? HeatingType { get; init; }

        /// <summary>
        /// Состояние объекта.
        /// </summary>
        /// <example>Новостройка</example>
        public string? PropertyCondition { get; init; }

        /// <summary>
        /// Наличие парковки.
        /// </summary>
        /// <example>true</example>
        public bool? HasParking { get; init; }

        /// <summary>
        /// Номер страницы для пагинации.
        /// </summary>
        /// <example>1</example>
        public int Page { get; init; } = 1;

        /// <summary>
        /// Количество элементов на странице.
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; init; } = 10;

        /// <summary>
        /// Поле для сортировки.
        /// </summary>
        /// <example>Price</example>
        public string? SortBy { get; init; }

        /// <summary>
        /// Порядок сортировки ("asc" для возрастания, "desc" для убывания).
        /// </summary>
        /// <example>desc</example>
        public string SortOrder { get; init; } = "asc";
    }

    /// <summary>
    /// Ответ на запрос поиска объектов недвижимости.
    /// </summary>
    /// <param name="Items">Коллекция найденных объектов недвижимости.</param>
    /// <param name="TotalCount">Общее количество объектов, соответствующих критериям поиска.</param>
    /// <param name="PageSize">Размер страницы.</param>
    /// <param name="TotalPages">Общее количество страниц.</param>
    public sealed record SearchPropertiesQueryResponse(
        IEnumerable<PropertyDto> Items,
        int TotalCount,
        int PageSize,
        int TotalPages
    );
}