namespace BlogApp.Core.Dtos.PagedFilters;

public class CommentPagedRequest : PagedRequestBase
{
    public bool? IsEdited  {get; set;}
    public Guid? PostId { get; set; }
    public Guid? UserId { get; set; }
}