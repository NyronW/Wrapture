namespace Wrapture.Pagination;

[Serializable]
public class PagedResult<T>(IEnumerable<T> items, int totalRecords, int currentPage, int pageSize = 10)
{
    public IEnumerable<T> Items { get; set; } = items;
    public Pager Pager { get; set; } = new(totalRecords, currentPage, pageSize);

    public PagedResult<TTarget> To<TTarget>(Func<IEnumerable<T>, IEnumerable<TTarget>> converter)
    {
        var items = converter(Items);

        return new PagedResult<TTarget>(items, Pager.TotalRecords, Pager.CurrentPage, Pager.PageSize);
    }

    public async Task<PagedResult<TTarget>> ToAsync<TTarget>(Func<IEnumerable<T>, Task<IEnumerable<TTarget>>> converter)
    {
        var convertedItems = await converter(Items);
        return new PagedResult<TTarget>(convertedItems, Pager.TotalRecords, Pager.CurrentPage, Pager.PageSize);
    }

}

public class Pager
{
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 10;
    public int TotalRecords { get; set; }

    // Calculated fields for record range
    public int StartRecordIndex { get; private set; }
    public int EndRecordIndex { get; private set; }

    public Pager(int totalRecords, int currentPage, int pageSize)
    {
        TotalRecords = totalRecords;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        CalculateRecordRange();
    }

    private void CalculateRecordRange()
    {
        StartRecordIndex = (CurrentPage - 1) * PageSize + 1;
        EndRecordIndex = Math.Min(CurrentPage * PageSize, TotalRecords);
    }
}