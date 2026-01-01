using System.ComponentModel.DataAnnotations;

namespace Presenter.DTOs.PropertyDTO;

public record ApiAddressDto(
    [Required] string Street,
    [Required] string City,
    [Required] int HomeNumber,
    [Required] int ZipCode,
    [Required] string Country
);