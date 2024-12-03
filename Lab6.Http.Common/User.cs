namespace Lab6.Http.Common;

public class TaskItem
{
    public TaskItem()
    {
    }

    public TaskItem(int id, string title, string description, string status, string priority, DateTime dueDate)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
    }

    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "Новая";

    public string Priority { get; set; } = "Средний";

    public DateTime DueDate { get; set; }
}
