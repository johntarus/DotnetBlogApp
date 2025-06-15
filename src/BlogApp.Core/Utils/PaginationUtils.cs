using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Core.Utils;

public static class PaginationUtils
{
    public static async Task<PaginatedList<T>> CreateAsync<T>
        (IQueryable<T> query, int pageNumber, int PageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        var items = await query.Skip((pageNumber - 1) * PageSize).Take(PageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, pageNumber, PageSize, totalCount, totalPages);
    }
}