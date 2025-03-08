namespace Wrapture.Pagination;

public static class Extensions
{
    public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, int totalRecords, int currentPage, int pageSize)
    {
        return new PagedResult<T>(source.Skip((currentPage - 1) * pageSize), totalRecords, currentPage, pageSize);
    }

    public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> query,
                                                  int currentPage,
                                                  int pageSize)
    {
        currentPage.LessThan(1, "Current page must be at least 1.");
        pageSize.LessThan(1, "Page size must be at least 1.");

        var totalRecords = query.Count();
        var items = query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>(items, totalRecords, currentPage, pageSize);
    }

    public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> query,
                                                  int currentPage,
                                                  int pageSize,
                                                  Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        currentPage.LessThan(1, "Current page must be at least 1.");
        pageSize.LessThan(1, "Page size must be at least 1.");

        if (orderBy != null) query = orderBy(query);

        var totalRecords = query.Count();
        var items = query
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<T>(items, totalRecords, currentPage, pageSize);
    }
}
