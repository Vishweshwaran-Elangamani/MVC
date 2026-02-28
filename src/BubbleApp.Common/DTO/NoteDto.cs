namespace BubbleApp.Common.DTO;

public record NoteDto(string Id, string Workspace, string UserId, string Content, DateTime CreatedAt);