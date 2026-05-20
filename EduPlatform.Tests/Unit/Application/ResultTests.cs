using EduPlatform.Application.Common.Models;

namespace EduPlatform.Tests.Unit.Application;

/// <summary>
/// Unit tests for the Result&lt;T&gt; and PagedResult&lt;T&gt; model classes.
///
/// What is Result&lt;T&gt;?
/// Result&lt;T&gt; is the service return type for operations that may fail.
/// Instead of throwing exceptions for expected failures (not found, bad request),
/// services return a Result object. Controllers read IsSuccess and StatusCode
/// to decide what HTTP response to return.
///
/// Why test this?
/// The Result class is used by every single service method in the application.
/// A bug in Result (e.g. Success returning IsSuccess=false) would silently
/// break every API endpoint. These tests verify the factory methods produce
/// the correct combination of IsSuccess, StatusCode, Data, and Error.
///
/// PagedResult&lt;T&gt; tests verify:
/// - TotalPages calculation is correct (ceiling division)
/// - HasNextPage / HasPreviousPage flags are accurate
/// These are used on every list page — incorrect flags would show wrong
/// pagination controls to users.
/// </summary>
public class ResultTests
{
    // ── Result&lt;T&gt; tests ──────────────────────────────────────────────────────

    /// <summary>
    /// The happy path: Success() must set IsSuccess=true, populate Data,
    /// set StatusCode=200, and leave Error=null.
    /// </summary>
    [Fact]
    public void Success_Result_IsSuccess_True()
    {
        var result = Result<string>.Success("data");

        Assert.True(result.IsSuccess);
        Assert.Equal("data", result.Data);
        Assert.Equal(200, result.StatusCode);
        Assert.Null(result.Error);
    }

    /// <summary>
    /// Failure() must set IsSuccess=false, populate Error, set the given StatusCode,
    /// and leave Data=null (never return partial data on failure).
    /// </summary>
    [Fact]
    public void Failure_Result_IsSuccess_False()
    {
        var result = Result<string>.Failure("Something went wrong", 400);

        Assert.False(result.IsSuccess);
        Assert.Equal("Something went wrong", result.Error);
        Assert.Equal(400, result.StatusCode);
        Assert.Null(result.Data);
    }

    /// <summary>
    /// NotFound() is shorthand for Failure with 404.
    /// Controllers use result.StatusCode == 404 to return NotFound() responses.
    /// </summary>
    [Fact]
    public void NotFound_Result_Has_404_StatusCode()
    {
        var result = Result<string>.NotFound("Not found");

        Assert.False(result.IsSuccess);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Not found", result.Error);
    }

    // ── PagedResult&lt;T&gt; tests ────────────────────────────────────────────────

    /// <summary>
    /// TotalPages must be ceil(TotalCount / PageSize).
    /// 25 items ÷ 5 per page = exactly 5 pages.
    /// On page 1 of 5: HasNextPage=true (there are pages 2–5), HasPreviousPage=false.
    /// </summary>
    [Fact]
    public void PagedResult_TotalPages_Calculates_Correctly()
    {
        var paged = PagedResult<int>.Create(
            items:       Enumerable.Range(1, 5).ToList(),
            totalCount:  25,
            pageNumber:  1,
            pageSize:    5);

        Assert.Equal(5, paged.TotalPages);
        Assert.True(paged.HasNextPage);
        Assert.False(paged.HasPreviousPage);
    }
}
