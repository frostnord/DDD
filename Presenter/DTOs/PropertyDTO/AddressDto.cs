using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO;

public record AddressDto(
    [Required] string Street,
    [Required] string City,
    [Required] int HomeNumber,
    [Required] int ZipCode,
    [Required] string Country
);