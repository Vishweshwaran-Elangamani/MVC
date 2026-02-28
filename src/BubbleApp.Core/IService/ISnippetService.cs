using BubbleApp.Common.ViewModels.Snippet;

namespace BubbleApp.Core.IService
{
    public interface ISnippetService
    {
        SnippetResponse Generate(string workspaceSlug, Uri widgetCdnUrl);
    }
}