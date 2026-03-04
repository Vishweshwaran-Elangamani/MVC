using System;
using BubbleApp.Common.ViewModels.Snippet;
using BubbleApp.Core.IService;
using BubbleApp.Data.IRepository;

namespace BubbleApp.Core.Service
{
  public class SnippetService : ISnippetService
  {
    private readonly IWorkspaceRepository _workspaces;

    public SnippetService(IWorkspaceRepository workspaces)
    {
      _workspaces = workspaces;
    }

    public SnippetResponse Generate(string workspaceSlug, Uri widgetCdnUrl)
    {
      var ws = _workspaces
          .GetBySlugAsync(workspaceSlug)
          .GetAwaiter()
          .GetResult()
          ?? throw new InvalidOperationException($"Workspace '{workspaceSlug}' not found.");

      var workspaceKey = ws.WorkspaceKey;

      var snippet = $@"
<script>
window.BUBBLE_USER = {{
  workspace: ""{workspaceSlug}"",
  key: ""{workspaceKey}""
}};
</script>
 
<script src=""{widgetCdnUrl}""></script>
";

      return new SnippetResponse(workspaceSlug, snippet);
    }
  }
}
