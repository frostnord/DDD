namespace Presenter.DTOs.PropertyDTO;

public sealed record AdressDTO
{
    public string Street { get; set; }
    public string City { get; set; }
    public int HomeNumber { get; set; }
    public int ZipCode { get; set; }
    public string Country { get; set; }
}