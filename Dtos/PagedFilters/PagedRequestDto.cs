namespace BlogApp.Dtos.PagedFilters;

public class PagedRequestBase
{
    public string? SearchQuery { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}