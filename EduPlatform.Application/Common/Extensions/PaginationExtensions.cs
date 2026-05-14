using EduPlatform.Application.Common.Models;

namespace EduPlatform.Application.Common.Extensions;

public static class PaginationExtensions
{
    public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 20 : pageSize;
        var list = source.ToList();
        var items = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return PagedResult<T>.Create(items, list.Count, pageNumber, pageSize);
    }
}
