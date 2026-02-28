namespace BubbleApp.Common.ViewModels.Notes;

public record NoteDto(string Id, string Workspace, string UserId, string Content, DateTime CreatedAt);