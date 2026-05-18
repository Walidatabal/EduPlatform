namespace EduPlatform.Application.Common.Models;

/// <summary>
/// Paginated result wrapper for list endpoints.
///
/// Why paginate?
/// Enterprise systems must never return unbounded result sets.
/// A table with 50,000 courses would bring down the server and crash the browser
/// if returned in one response. Pagination limits results to a manageable page
/// and provides the client with metadata to render pagination controls.
///
/// Usage in API controllers:
/// <code>
/// var paged = await courseService.GetPublishedAsync(query.PageNumber, query.PageSize);
/// return Ok(ApiResponse.Ok(paged));
/// </code>
///
/// The client uses TotalPages, HasNextPage, HasPreviousPage to render
/// "Page 3 of 12" controls and enable/disable Next/Previous buttons.
///
/// Factory method:
/// <code>
/// var paged = PagedResult&lt;CourseListDto&gt;.Create(
///     items:       courseDtos,
///     totalCount:  totalRecords,  // from COUNT(*) query
///     pageNumber:  query.Page,
///     pageSize:    query.PageSize);
/// </code>
/// </summary>
/// <typeparam name="T">The item type in the paginated list.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The items for the current page.
    /// Count will be ≤ PageSize (the last page may have fewer items).
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Total number of records across all pages (from the COUNT query).
    /// Used to calculate TotalPages and render "Showing 1–10 of 247 results".
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number. 1-indexed (first page = 1, not 0).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Maximum number of items per page.
    /// Typical values: 10, 20, 50. Never accept unbounded page sizes from clients.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages = ceil(TotalCount / PageSize).
    /// Computed property — not stored in the database.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// True if there is a page before the current one.
    /// False when PageNumber = 1 (the first page).
    /// Use to enable/disable the Previous button on the client.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// True if there is another page after the current one.
    /// False when PageNumber = TotalPages (the last page).
    /// Use to enable/disable the Next button on the client.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Creates a PagedResult from a pre-fetched page of items and total count.
    /// The items list should already be the correct page (LINQ Skip/Take applied).
    /// The totalCount is the result of COUNT(*) before Skip/Take.
    /// </summary>
    public static PagedResult<T> Create(
        IReadOnlyList<T> items,
        int totalCount,
        int pageNumber,
        int pageSize)
        => new()
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
}
