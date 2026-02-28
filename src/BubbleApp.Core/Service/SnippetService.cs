using BubbleApp.Common.ViewModels.Snippet;
using BubbleApp.Core.IService;

namespace BubbleApp.Core.Service
{
    public class SnippetService : ISnippetService
    {
        public SnippetResponse Generate(string workspaceSlug, Uri widgetCdnUrl)
        {
            // Generic resolver (works for most apps that store user in localStorage).
            // Customers can customize it to their own storage pattern.
            var snippet = $@"<script>
  (function() {{
    function safeParse(j) {{ try {{ return JSON.parse(j); }} catch {{ return null; }} }}
    var u  = safeParse(localStorage.getItem('user'));
    var id = (u && (u.userId || u.id)) || localStorage.getItem('userId') || '';
    var email = (u && u.email) || localStorage.getItem('email') || '';
    window.BUBBLE_USER = {{ id: id, email: email, workspace: '{workspaceSlug}' }};
  }})();
</script>
<script src=""{widgetCdnUrl}""></script>";

            return new SnippetResponse(workspaceSlug, snippet);
        }
    }
}