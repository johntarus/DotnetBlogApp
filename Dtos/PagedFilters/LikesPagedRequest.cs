namespace BlogApp.Dtos.PagedFilters;

public class LikesPagedRequest : PagedRequestBase
{
    public Guid? PostId { get; set; }
    public Guid? UserId { get; set; }
}