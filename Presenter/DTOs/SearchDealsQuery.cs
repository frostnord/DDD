using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs
{
    /// <summary>
    /// Параметры поиска и фильтрации сделок
    /// </summary>
    public class SearchDealsQuery
    {
        /// <summary>
        /// Фильтр по идентификатору клиента
        /// </summary>
        public Guid? ClientId { get; set; }
        
        /// <summary>
        /// Фильтр по идентификатору объекта недвижимости
        /// </summary>
        public Guid? PropertyId { get; set; }
        
    }
}