using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Mapper;

namespace AppDiv.CRVS.Application.Common;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageCount { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, int pageCount, int pageSize)
    {
        PageCount = pageCount;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageCount > 1;

    public bool HasNextPage => PageCount < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageCount, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageCount - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pageCount, pageSize);
    }
    public static async Task<PaginatedList<T>> CreateAsync(List<T> source, int pageCount, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageCount - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedList<T>(items, count, pageCount, pageSize);
    }
}
