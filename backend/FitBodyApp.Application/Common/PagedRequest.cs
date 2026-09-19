using Microsoft.EntityFrameworkCore;

namespace FitBodyApp.Application.Common;

public class PagedRequest
{
    private int _page = 1;
    private int _limit = 20;

    public int Page { get => _page; set => _page = value < 1 ? 1 : value; }
    public int Limit { get => _limit; set => _limit = value is < 1 or > 100 ? 20 : value; }
}

public static class PagingExtensions
{
    public static async Task<(List<T> Items, PageMeta Meta)> ToPagedResultAsync<T>(
        this IQueryable<T> query, int page, int limit)
    {
        var total = await query.CountAsync();
        var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

        var meta = new PageMeta
        {
            Total = total,
            Page = page,
            Limit = limit,
            TotalPages = (int)Math.Ceiling(total / (double)limit)
        };
        return (items, meta);
    }
}
