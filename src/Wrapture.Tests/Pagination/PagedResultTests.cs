using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Wrapture.Pagination;

namespace Wrapture.Tests.Pagination
{
    public class PagedResultTests
    {
        [Fact]
        public void PagedResult_Constructor_Should_Initialize_Properties_Correctly()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 4, 5 };
            var totalRecords = 20;
            var currentPage = 2;
            var pageSize = 5;

            // Act
            var pagedResult = new PagedResult<int>(items, totalRecords, currentPage, pageSize);

            // Assert
            pagedResult.Items.Should().BeEquivalentTo(items);
            pagedResult.Pager.TotalRecords.Should().Be(totalRecords);
            pagedResult.Pager.CurrentPage.Should().Be(currentPage);
            pagedResult.Pager.PageSize.Should().Be(pageSize);
            pagedResult.Pager.TotalPages.Should().Be(4);
            pagedResult.Pager.StartRecordIndex.Should().Be(6);
            pagedResult.Pager.EndRecordIndex.Should().Be(10);
        }

        [Fact]
        public void PagedResult_Should_Calculate_TotalPages_Correctly()
        {
            // Arrange & Act
            var result1 = new PagedResult<int>(new[] { 1, 2, 3 }, 10, 1, 3);
            var result2 = new PagedResult<int>(new[] { 1, 2, 3 }, 11, 1, 3);
            var result3 = new PagedResult<int>(new[] { 1, 2, 3 }, 0, 1, 3);

            // Assert
            result1.Pager.TotalPages.Should().Be(4); // 10 / 3 = 3.33 -> 4 pages
            result2.Pager.TotalPages.Should().Be(4); // 11 / 3 = 3.67 -> 4 pages
            result3.Pager.TotalPages.Should().Be(0); // 0 / anything = 0 pages
        }

        [Fact]
        public void Pager_Should_Calculate_Record_Ranges_Correctly()
        {
            // Arrange & Act
            var result1 = new PagedResult<int>(new[] { 1, 2, 3 }, 25, 1, 10); // First page
            var result2 = new PagedResult<int>(new[] { 1, 2, 3 }, 25, 2, 10); // Middle page
            var result3 = new PagedResult<int>(new[] { 1, 2, 3 }, 25, 3, 10); // Last page (partial)

            // Assert
            // First page: records 1-10
            result1.Pager.StartRecordIndex.Should().Be(1);
            result1.Pager.EndRecordIndex.Should().Be(10);

            // Second page: records 11-20
            result2.Pager.StartRecordIndex.Should().Be(11);
            result2.Pager.EndRecordIndex.Should().Be(20);

            // Third page: records 21-25 (partial page)
            result3.Pager.StartRecordIndex.Should().Be(21);
            result3.Pager.EndRecordIndex.Should().Be(25);
        }

        [Fact]
        public void PagedResult_HasPrevious_Should_Be_Determined_Correctly()
        {
            // Arrange & Act
            var result1 = new PagedResult<int>(new[] { 1, 2, 3 }, 10, 1, 3);
            var result2 = new PagedResult<int>(new[] { 1, 2, 3 }, 10, 2, 3);

            // Assert
            // HasPrevious isn't directly in Pager - needs to be calculated
            (result1.Pager.CurrentPage > 1).Should().BeFalse(); // First page has no previous
            (result2.Pager.CurrentPage > 1).Should().BeTrue();  // Second page has previous
        }

        [Fact]
        public void PagedResult_HasNext_Should_Be_Determined_Correctly()
        {
            // Arrange & Act
            var result1 = new PagedResult<int>(new[] { 1, 2, 3 }, 10, 4, 3);  // Last page
            var result2 = new PagedResult<int>(new[] { 1, 2, 3 }, 10, 3, 3);  // Not last page

            // Assert
            // HasNext isn't directly in Pager - needs to be calculated
            (result1.Pager.CurrentPage < result1.Pager.TotalPages).Should().BeFalse(); // Last page has no next
            (result2.Pager.CurrentPage < result2.Pager.TotalPages).Should().BeTrue();  // Not last page has next
        }

        [Fact]
        public void ToPagedResult_For_IEnumerable_Should_Create_Correct_PagedResult()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            var totalRecords = source.Count;
            var currentPage = 2;
            var pageSize = 5;

            // Act
            var pagedResult = source.ToPagedResult(totalRecords, currentPage, pageSize);

            // Assert
            pagedResult.Items.Count().Should().Be(5);
            pagedResult.Pager.TotalRecords.Should().Be(15);
            pagedResult.Pager.CurrentPage.Should().Be(2);
            pagedResult.Pager.PageSize.Should().Be(5);
            pagedResult.Pager.TotalPages.Should().Be(3);
            pagedResult.Pager.StartRecordIndex.Should().Be(6);
            pagedResult.Pager.EndRecordIndex.Should().Be(10);
        }

        [Fact]
        public void ToPagedResult_For_IQueryable_Should_Create_Correct_PagedResult()
        {
            // Arrange
            var source = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }.AsQueryable();
            var currentPage = 2;
            var pageSize = 3;

            // Act
            var pagedResult = source.ToPagedResult(currentPage, pageSize);

            // Assert
            pagedResult.Items.Should().BeEquivalentTo(new[] { 4, 5, 6 }); // Second page with size 3
            pagedResult.Pager.TotalRecords.Should().Be(10);
            pagedResult.Pager.CurrentPage.Should().Be(currentPage);
            pagedResult.Pager.PageSize.Should().Be(pageSize);
            pagedResult.Pager.TotalPages.Should().Be(4); // 10 items / 3 per page = 4 pages
        }

        [Fact]
        public void ToPagedResult_For_IQueryable_With_OrderBy_Should_Create_Correct_PagedResult()
        {
            // Arrange
            var source = new List<int> { 5, 3, 1, 4, 2, 8, 6, 7, 9, 10 }.AsQueryable();
            var currentPage = 2;
            var pageSize = 3;

            // Act
            var pagedResult = source.ToPagedResult(
                currentPage,
                pageSize,
                q => q.OrderBy(x => x));

            // Assert
            pagedResult.Items.Should().BeEquivalentTo(new[] { 4, 5, 6 }); // Second page with ordered items
            pagedResult.Pager.TotalRecords.Should().Be(10);
            pagedResult.Pager.CurrentPage.Should().Be(currentPage);
            pagedResult.Pager.PageSize.Should().Be(pageSize);
            pagedResult.Pager.TotalPages.Should().Be(4);
        }

        [Fact]
        public void PagedResult_To_Should_Convert_To_New_Type()
        {
            // Arrange
            var pagedResult = new PagedResult<int>(new[] { 1, 2, 3 }, 10, 1, 5);

            // Act
            var converted = pagedResult.To(items => items.Select(i => i.ToString()));

            // Assert
            converted.Items.Should().BeEquivalentTo(new[] { "1", "2", "3" });
            converted.Pager.TotalRecords.Should().Be(10);
            converted.Pager.CurrentPage.Should().Be(1);
            converted.Pager.PageSize.Should().Be(5);
        }

        [Fact]
        public async Task PagedResult_ToAsync_Should_Convert_To_New_Type_Asynchronously()
        {
            // Arrange
            var pagedResult = new PagedResult<int>(new[] { 1, 2, 3 }, 10, 1, 5);

            // Act
            var converted = await pagedResult.ToAsync(async items =>
            {
                await Task.Delay(1);
                return items.Select(i => i.ToString());
            });

            // Assert
            converted.Items.Should().BeEquivalentTo(new[] { "1", "2", "3" });
            converted.Pager.TotalRecords.Should().Be(10);
            converted.Pager.CurrentPage.Should().Be(1);
            converted.Pager.PageSize.Should().Be(5);
        }
    }
}
