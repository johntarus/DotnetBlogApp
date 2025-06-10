namespace BlogApp.Dtos.Request;

public class PagedRequestDto
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchQuery { get; set; }
}