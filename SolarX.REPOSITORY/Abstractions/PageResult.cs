using Microsoft.EntityFrameworkCore;

namespace SolarX.REPOSITORY.Abstractions;

public class PagedResult<T>
{
    public const int UpperPageSize = 100;
    public const int DefaultPageSize = 10;
    public const int DefaultPageIndex = 1;
    
    public PagedResult(List<T> items, int pageIndex, int pageSize, int totalCount)
    {
        Items = items;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public List<T> Items { get; }
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    
    public bool HasNextPage => PageIndex < (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    
    public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize)
    {
        pageIndex = pageIndex <= 0 ? DefaultPageIndex : pageIndex;
        pageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, UpperPageSize);

        // Đảm bảo OrderBy được áp dụng
        var entityType = typeof(T);
        if (entityType.GetProperty("CreatedAt") != null)
        {
            query = query.OrderByDescending(x => EF.Property<object>(x, "CreatedAt"));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        pageIndex = Math.Min(pageIndex, totalPages > 0 ? totalPages : 1);

        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResult<T>(items, pageIndex, pageSize, totalCount);
    }
}