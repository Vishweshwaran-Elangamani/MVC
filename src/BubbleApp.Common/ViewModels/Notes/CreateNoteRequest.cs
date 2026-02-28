namespace BubbleApp.Common.ViewModels.Notes;

public record CreateNoteRequest(string Workspace, string UserId, string Content);