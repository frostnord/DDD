using System;

namespace Presenter.DTOs.SellerDTO
{
    /// <summary>
    /// DTO для представления продавца
    /// </summary>
    public class SellerDto
    {
        public Guid Id { get; set; }
        
        public Guid ClientId { get; set; }
        
        public DateTime RegisteredAt { get; set; }
    }
}