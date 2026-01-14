namespace Presenter.DTOs.PropertyDTO.Response
{
    public record PagedPropertiesResponse(
        IEnumerable<PropertyResponse> Items,
        int TotalCount,
        int PageSize,
        int TotalPages,
        int CurrentPage
    );
}