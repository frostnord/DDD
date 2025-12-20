namespace Presenter.DTOs.PropertyDTO
{
    /// <summary>
    /// DTO для недвижимости
    /// </summary>
    public class PropertyDto
    {
        public Guid Id { get; set; }
        
        public string Address { get; set; }
        
        public decimal Price { get; set; }
        
        public string Status { get; set; }
        
        public string Description { get; set; }
        
        public int NumberOfRooms { get; set; }
        
        public int Floor { get; set; }
        
        public int TotalFloors { get; set; }
        
        public string PropertyType { get; set; }
        
        public string HeatingType { get; set; }
        
        public string PropertyCondition { get; set; }
        
        public double Area { get; set; }
        
        public bool? HasParking { get; set; }
        
        public Guid OwnerClientId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}