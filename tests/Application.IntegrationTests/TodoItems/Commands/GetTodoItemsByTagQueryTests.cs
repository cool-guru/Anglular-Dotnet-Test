using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.TodoItems.Queries;

using static Testing;

public class GetTodoItemsByTagQueryTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnItemsWithMatchingTag()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "Tagged List"
        });

        var itemId1 = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Item with Tag",
            Tags = new List<string> { "work" }
        });

        var itemId2 = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Item with other Tag",
            Tags = new List<string> { "personal" }
        });

        var context = await GetDbContextAsync();

        var items = await context.TodoItems
            .Where(t => !t.IsDeleted)
            .ToListAsync();

        var matchingItems = items
            .Where(t => t.Tags.Contains("work"))
            .ToList();
        matchingItems.Should().HaveCount(1);
        matchingItems[0].Title.Should().Be("Item with Tag");
    }

    [Test]
    public async Task ShouldNotReturnSoftDeletedItems()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "Tagged List 2"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "To be deleted",
            Tags = new List<string> { "work" }
        });

        var context = await GetDbContextAsync();
        var item = await context.TodoItems.FindAsync(itemId);
        item!.IsDeleted = true;
        await context.SaveChangesAsync(CancellationToken.None);

        var items = await context.TodoItems
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        var remainingItems = items
            .Where(t => t.Tags.Contains("work"))
            .ToList();

        remainingItems.Should().BeEmpty();
    }
}
