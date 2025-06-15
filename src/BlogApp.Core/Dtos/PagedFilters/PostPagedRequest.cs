namespace BlogApp.Core.Dtos.PagedFilters;

public class PostPagedRequest : PagedRequestBase
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public int? CategoryId { get; set; }
}