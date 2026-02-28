namespace BubbleApp.Common.Entities;

public class Workspace
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string AdminId { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}