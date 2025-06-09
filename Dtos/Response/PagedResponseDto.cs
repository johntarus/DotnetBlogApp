namespace BlogApp.Dtos.Response;

public class PagedResponseDto<T>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public List<T> Items { get; init; }
}