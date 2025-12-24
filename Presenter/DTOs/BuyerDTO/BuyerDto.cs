using System;

namespace Presenter.DTOs.BuyerDTO
{
    /// <summary>
    /// DTO для представления покупателя
    /// </summary>
    public class BuyerDto
    {
        public Guid Id { get; set; }
        
        public Guid ClientId { get; set; }
        
        public DateTime RegisteredAt { get; set; }
    }
}