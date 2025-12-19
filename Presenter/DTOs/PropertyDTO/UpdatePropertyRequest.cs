using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO
{
    public class UpdatePropertyRequest
    {
        [Required] public string Street { get; set; }

        [Required] public string City { get; set; }

        [Required] public int HomeNumber { get; set; }

        [Required] public int ZipCode { get; set; }

        [Required] public string Country { get; set; }

        [Required][Range(0, double.MaxValue)] public decimal Price { get; set; }

        [Required][StringLength(1000)] public string Description { get; set; }

        [Required] public int NumberOfRooms { get; set; }

        [Required] public int Floor { get; set; }

        [Required] public int TotalFloors { get; set; }

        [Required] public string PropertyType { get; set; }

        [Required] public string HeatingType { get; set; }

        [Required] public string PropertyCondition { get; set; }

        [Required] public double Area { get; set; }

        public bool? HasParking { get; set; }
    }
}