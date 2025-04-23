namespace Todo_App.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    public string? Title { get; set; }
    public string Colour { get; set; } = "#ffffff";
    public bool IsDeleted { get; set; } = false;
    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
