using FluentAssertions;
using NUnit.Framework;
using Todo_App.Application.TodoItems.Commands.CreateTodoItem;
using Todo_App.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Todo_App.Application.TodoLists.Commands.CreateTodoList;
using Todo_App.Domain.Entities;
using Todo_App.Domain.Enums;

namespace Todo_App.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemDetailWithTagsAndColorTests : BaseTestFixture
{
    [Test]
    public async Task ShouldUpdateTagsAndColor()
    {
        var listId = await SendAsync(new CreateTodoListCommand { Title = "ColorTagList" });

        var itemId = await SendAsync(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "Styled Task"
        });

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = listId,
            Note = "Detailed with style",
            Priority = PriorityLevel.Low,
            Tags = new List<string> { "urgent", "home" },
            Colour = "#ff00ff"
        };

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);
        item.Should().NotBeNull();
        item!.Tags.Should().BeEquivalentTo(command.Tags);
        item.Colour.Should().Be(command.Colour);
    }
}
