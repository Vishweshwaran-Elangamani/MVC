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
            => _workspaces = workspaces;
 
        public SnippetResponse Generate(string workspaceSlug, Uri widgetCdnUrl)
        {
            var ws = _workspaces
                .GetBySlugAsync(workspaceSlug)
                .GetAwaiter()
                .GetResult()
                ?? throw new InvalidOperationException($"Workspace '{workspaceSlug}' not found.");
 
            var workspaceKey = ws.WorkspaceKey;
 
            var snippet = $$"""
<script>
(function () {
 
  var WIDGET_URL     = "{{widgetCdnUrl}}";
  var WORKSPACE      = "{{workspaceSlug}}";
  var WORKSPACE_KEY  = "{{workspaceKey}}";
 
  var LOGIN_HINTS = ["/login", "/signin", "/auth"];
  var POLL_MS = 1500;
 
  var workspaceDisabled = false;
 
  function safeParse(j){ try { return JSON.parse(j); } catch { return null; } }
  function pick(o,p){ if(!o||!p) return; return p.split('.').reduce(function(a,k){return a&&a[k];}, o); }
  function getCookie(n){ var m=document.cookie.split('; ').find(c=>c.indexOf(n+'=')===0); return m?m.split('=')[1]:''; }
  function meta(name){ var el=document.querySelector('meta[name="'+name+'"]'); return el?el.content:''; }
  function qp(name){ return new URLSearchParams(location.search).get(name) || ''; }
 
  var GLOBAL_VAR="AppUser", REDUX_STORE_VAR="__REDUX_STORE__", REDUX_ID="auth.user.id", REDUX_EM="auth.user.email";
  var LS_USER="user", LS_UID="userId", LS_EM="email", ID="id", ALTID="userId", EMAIL="email";
 
  function getUser(){
    var g = window[GLOBAL_VAR];
    if(g){
      var gid=(g[ID]||g[ALTID]||"").toString().trim();
      var gem=g[EMAIL]||"";
      if(gid) return {id:gid,email:gem};
    }
 
    var store = window[REDUX_STORE_VAR];
    if(store && typeof store.getState==="function"){
      var st=store.getState();
      var rid=(pick(st,REDUX_ID)||"").toString().trim();
      var rem=pick(st,REDUX_EM)||"";
      if(rid) return {id:rid,email:rem};
    }
 
    var u=safeParse(localStorage.getItem(LS_USER));
    var lid=(u&&(u[ALTID]||u[ID])) || localStorage.getItem(LS_UID) || "";
    var lem=(u&&u[EMAIL]) || localStorage.getItem(LS_EM) || "";
    lid=(lid||"").toString().trim();
    if(lid) return {id:lid,email:lem};
 
    var cid=decodeURIComponent(getCookie(LS_UID)||"");
    var cem=decodeURIComponent(getCookie(LS_EM)||"");
    cid=(cid||"").toString().trim();
    if(cid) return {id:cid,email:cem};
 
    var mid=(meta("bubble:userId")||"").toString().trim();
    var mem=meta("bubble:email")||"";
    if(mid) return {id:mid,email:mem};
 
    var qid=(qp("bubbleUserId")||"").toString().trim();
    var qem=qp("bubbleEmail")||"";
    if(qid) return {id:qid,email:qem};
 
    return { id:"", email:"" };
  }
 
  function onLoginPage(){
    var p=(location.pathname||"").toLowerCase();
    return LOGIN_HINTS.some(function(h){ return p.indexOf(h)>=0; });
  }
 
  function bubbleExists(){
    return !!document.getElementById("bubble-btn");
  }
 
  function panelIsOpen(){
    var panel=document.getElementById("bubble-panel");
    return !!panel && panel.style && panel.style.display!=="none";
  }
 
  function teardown(){
    var btn=document.getElementById("bubble-btn");
    var panel=document.getElementById("bubble-panel");
    if(btn && btn.parentNode) btn.parentNode.removeChild(btn);
    if(panel && panel.parentNode) panel.parentNode.removeChild(panel);
  }
 
  function injectWidget(user){
 
    if(workspaceDisabled) return;
 
    Array.prototype.slice.call(document.querySelectorAll('script[src]'))
      .filter(function(s){ return (s.src||'').indexOf('widget.js')>=0; })
      .forEach(function(s){ s.parentNode && s.parentNode.removeChild(s); });
 
    window.BUBBLE_USER = {
      id: user.id || "",
      email: user.email || "",
      workspace: WORKSPACE,
      key: WORKSPACE_KEY
    };
 
    var s=document.createElement("script");
    s.src=WIDGET_URL;
    s.async=true;
 
    s.onerror=function(){
      workspaceDisabled = true;
      teardown();
    };
 
    document.body.appendChild(s);
  }
 
  var lastUserId=null, lastPath=null;
 
  function evaluate(){
 
    if(workspaceDisabled) return;
 
    var path=location.pathname||"";
    var isLogin=onLoginPage();
    var user=getUser();
 
    if(isLogin || !user.id){
      if(bubbleExists() && !panelIsOpen()) teardown();
      lastUserId=user.id||"";
      lastPath=path;
      return;
    }
 
    var needsUserChange=(lastUserId!==user.id);
    var needsPathChange=(lastPath!==path);
    var exists=bubbleExists();
 
    if((!exists && !workspaceDisabled) || needsUserChange){
 
      if(panelIsOpen() && !needsUserChange){
        return;
      }
 
      if(needsUserChange) teardown();
 
      injectWidget(user);
 
      lastUserId=user.id;
      lastPath=path;
    }
  }
 
  evaluate();
  setInterval(evaluate, POLL_MS);
 
})();
</script>
""";
 
            return new SnippetResponse(workspaceSlug, snippet);
        }
    }
}
 