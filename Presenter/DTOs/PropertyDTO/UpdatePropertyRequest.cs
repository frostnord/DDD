using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO
{
    public class UpdatePropertyRequest
    {
        [Required] public string Street { get; init; }

        [Required] public string City { get; init; }

        [Required] public int HomeNumber { get; init; }

        [Required] public int ZipCode { get; init; }

        [Required] public string Country { get; init; }

        [Required][Range(0, double.MaxValue)] public decimal Price { get; init; }

        [Required][StringLength(1000)] public string Description { get; init; }

        [Required] public int NumberOfRooms { get; init; }

        [Required] public int Floor { get; init; }

        [Required] public int TotalFloors { get; init; }

        [Required] public string PropertyType { get; init; }

        [Required] public string HeatingType { get; init; }

        [Required] public string PropertyCondition { get; init; }

        [Required] public decimal Area { get; init; }

        public bool? HasParking { get; init; }
    }
}