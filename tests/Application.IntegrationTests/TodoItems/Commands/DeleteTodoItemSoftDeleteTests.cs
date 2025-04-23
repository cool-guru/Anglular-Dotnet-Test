using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using Todo_App.Application.Common.Exceptions;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoItems.Commands.DeleteTodoItem;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Domain.Entities;

namespace Todo_App.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class DeleteTodoItemSoftDeleteTests : BaseTestFixture
{
    [Test]
    public async Task ShouldSoftDeleteTodoItem()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "SoftDelete Test List"
        });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "SoftDelete Test Item"
        });

        await SendAsync(new DeleteTodoItemCommand(itemId));

        var dbContext = await GetDbContextAsync();

        var item = await dbContext.TodoItems
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Id == itemId);

        item.Should().NotBeNull();
        item!.IsDeleted.Should().BeTrue();
    }

    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand(9999);
        await FluentActions.Invoking(() => SendAsync(command))
            .Should().ThrowAsync<NotFoundException>();
    }
}
