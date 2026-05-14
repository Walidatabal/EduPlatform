using EduPlatform.Application.Common.Models;
using Xunit;

namespace EduPlatform.Tests.Unit.Application;

public class ResultTests
{
    [Fact]
    public void Success_Result_IsSuccess_True()
    {
        var result = Result<string>.Success("data");
        Assert.True(result.IsSuccess);
        Assert.Equal("data", result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public void Failure_Result_IsSuccess_False()
    {
        var result = Result<string>.Failure("Something went wrong", 400);
        Assert.False(result.IsSuccess);
        Assert.Equal("Something went wrong", result.Error);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public void NotFound_Result_Has_404_StatusCode()
    {
        var result = Result<string>.NotFound("Not found");
        Assert.Equal(404, result.StatusCode);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void PagedResult_TotalPages_Calculates_Correctly()
    {
        var paged = PagedResult<int>.Create(
            items: Enumerable.Range(1, 5).ToList(),
            totalCount: 25, pageNumber: 1, pageSize: 5);

        Assert.Equal(5, paged.TotalPages);
        Assert.True(paged.HasNextPage);
        Assert.False(paged.HasPreviousPage);
    }
}
