using BubbleApp.Common.ViewModels.Snippet;
using BubbleApp.Core.IService;

namespace BubbleApp.Core.Service
{
    public class SnippetService : ISnippetService
    {
        public SnippetResponse Generate(string workspaceSlug, Uri widgetCdnUrl)
        {
            // Raw interpolated string (C# 11+): no escaping headaches, {{...}} is interpolation.
            var snippet = $$"""
<script>
(function () {
  // -------- Config --------
  var WIDGET_URL = "{{widgetCdnUrl}}";  // your widget path
  var WORKSPACE  = "{{workspaceSlug}}"; // your workspace slug
  var LOGIN_HINTS = ["/login", "/signin", "/auth"]; // hide on these paths
  var POLL_MS = 1500;                    // lightweight poll (does NOT reload each tick)

  // -------- Utilities --------
  function safeParse(j){ try { return JSON.parse(j); } catch { return null; } }
  function getUser() {
    var u  = safeParse(localStorage.getItem("user"));
    var id = (u && (u.userId || u.id)) || localStorage.getItem("userId") || "";
    var email = (u && u.email) || localStorage.getItem("email") || "";
    id = (id || "").toString().trim();
    return { id: id, email: email };
  }
  function onLoginPage() {
    var p = (location.pathname || "").toLowerCase();
    return LOGIN_HINTS.some(function(h){ return p.indexOf(h) >= 0; });
  }
  function bubbleExists() {
    return !!document.getElementById("bubble-btn") && !!document.getElementById("bubble-panel");
  }
  function panelIsOpen() {
    var panel = document.getElementById("bubble-panel");
    return !!panel && panel.style && panel.style.display !== "none";
  }
  function injectWidget() {
    // remove any old script tag (avoid duplicates)
    var olds = Array.prototype.slice.call(document.querySelectorAll("script[src]"))
      .filter(function(s){ return (s.src || "").indexOf("widget.js") >= 0; });
    olds.forEach(function(s){ s.parentNode && s.parentNode.removeChild(s); });

    var s = document.createElement("script");
    s.src = WIDGET_URL;
    s.async = true;
    s.onload = function(){ console.log("[Bubble] widget.js loaded"); };
    s.onerror = function(e){ console.warn("[Bubble] widget.js failed", e); };
    document.body.appendChild(s);
  }
  function setGlobalUser(u) {
    window.BUBBLE_USER = { id: u.id || "", email: u.email || "", workspace: WORKSPACE };
    console.log("[Bubble] BUBBLE_USER ->", window.BUBBLE_USER);
  }
  function teardownIfNeeded() {
    // Only used when we want to remove bubble on login pages or when user becomes empty.
    var btn = document.getElementById("bubble-btn");
    var panel = document.getElementById("bubble-panel");
    if (btn && btn.parentNode) btn.parentNode.removeChild(btn);
    if (panel && panel.parentNode) panel.parentNode.removeChild(panel);
  }

  // -------- Smart bootstrap (runs once) --------
  var lastUserId = null;
  var lastPath   = null;

  function evaluate() {
    var path = location.pathname || "";
    var isLogin = onLoginPage();
    var user = getUser();

    // 1) If on login page OR user id empty -> ensure bubble is gone (but do not reload)
    if (isLogin || !user.id) {
      if (bubbleExists()) {
        // Only tear down if NOT open, to avoid jarring UI while user is reading
        if (!panelIsOpen()) teardownIfNeeded();
      }
      lastUserId = user.id || "";
      lastPath = path;
      return;
    }

    // 2) We are on an app page and have a user id -> ensure bubble exists and is for this user
    var needsUserChange = (lastUserId !== user.id);
    var needsPathChange = (lastPath   !== path);
    var exists = bubbleExists();

    // If bubble doesn't exist, or user changed, or we navigated to a new path,
    // re-initialize the widget. If the panel is open, we avoid tearing it down
    // unless the user actually changed (to prevent closing while reading).
    if (!exists || needsUserChange || needsPathChange) {
      if (panelIsOpen() && !needsUserChange) {
        // Don't kill an open panel just because route changed: keep it.
        console.log("[Bubble] Route changed but panel open; skipping re-init.");
      } else {
        // Re-initialize cleanly for the current user
        // (Only remove nodes if they exist and we must switch users)
        if (needsUserChange) { teardownIfNeeded(); }
        setGlobalUser(user);
        if (!exists || needsUserChange) injectWidget();
      }
      lastUserId = user.id;
      lastPath = path;
      return;
    }

    // 3) Nothing to do: bubble exists, user & path unchanged
  }

  // Initial pass
  evaluate();

  // Lightweight polling: checks state but DOES NOT re-create every tick
  setInterval(evaluate, POLL_MS);

})();
</script>
http://localhost:3000/widget.js"></script>
""";

            return new SnippetResponse(workspaceSlug, snippet);
        }
    }
}