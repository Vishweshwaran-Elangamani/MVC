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
 
            var snippet = $$"""
<script>
(function () {
 
var WIDGET_URL = "{{widgetCdnUrl}}";
var WORKSPACE = "{{workspaceSlug}}";
var WORKSPACE_KEY = "{{workspaceKey}}";
 
if(window.__BUBBLE_DISABLED__) return;
 
var LOGIN_HINTS = ["/login", "/signin", "/auth"];
var workspaceDisabled = false;
 
/* ---------------- USER DETECTION ---------------- */
 
function safeParse(j){
 try { return JSON.parse(j); } catch { return null; }
}
 
function pick(o,p){
 if(!o||!p) return;
 return p.split('.').reduce(function(a,k){return a&&a[k];}, o);
}
 
function getCookie(n){
 var m=document.cookie.split('; ').find(function(c){
  return c.indexOf(n+'=')===0;
 });
 return m?m.split('=')[1]:'';
}
 
function meta(name){
 var el=document.querySelector('meta[name="'+name+'"]');
 return el?el.content:'';
}
 
function qp(name){
 return new URLSearchParams(location.search).get(name) || '';
}
 
var GLOBAL_VAR="AppUser";
var REDUX_STORE_VAR="__REDUX_STORE__";
var REDUX_ID="auth.user.id";
var REDUX_EM="auth.user.email";
 
var LS_USER="user";
var LS_UID="userId";
var LS_EM="email";
 
var ID="id";
var ALTID="userId";
var EMAIL="email";
 
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
 
/* ---------------- PAGE CHECK ---------------- */
 
function onLoginPage(){
 var p=(location.pathname||"").toLowerCase();
 return LOGIN_HINTS.some(function(h){
  return p.indexOf(h)>=0;
 });
}
 
function bubbleExists(){
 return !!document.getElementById("bubble-btn");
}
 
function panelIsOpen(){
 var panel=document.getElementById("bubble-panel");
 return !!panel && panel.style && panel.style.display!=="none";
}
 
/* ---------------- CLEANUP ---------------- */
 
function teardown(){
 
 var btn=document.getElementById("bubble-btn");
 var panel=document.getElementById("bubble-panel");
 
 if(btn && btn.parentNode) btn.parentNode.removeChild(btn);
 if(panel && panel.parentNode) panel.parentNode.removeChild(panel);
 
}
 
/* ---------------- WIDGET LOAD ---------------- */
 
function injectWidget(user){
 
 if(workspaceDisabled) return;
 if(window.__BUBBLE_DISABLED__) return;
 
 Array.prototype.slice.call(document.querySelectorAll('script[src]'))
  .filter(function(s){
   return (s.src||'').indexOf('widget.js')>=0;
  })
  .forEach(function(s){
   s.parentNode && s.parentNode.removeChild(s);
  });
 
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
 
/* ---------------- MAIN LOGIC ---------------- */
 
var lastUserId=null;
var lastPath=null;
 
function evaluate(){
 
 if(workspaceDisabled) return;
 if(window.__BUBBLE_DISABLED__) return;
 
 var path=location.pathname||"";
 var isLogin=onLoginPage();
 var user=getUser();
 
 if(isLogin || !user.id){
 
  if(bubbleExists()){
   teardown();
  }
 
  lastUserId="";
  lastPath=path;
 
  return;
 }
 
 var needsUserChange=(lastUserId!==user.id);
 var exists=bubbleExists();
 
 if((!exists && !workspaceDisabled) || needsUserChange){
 
  if(panelIsOpen() && !needsUserChange)
   return;
 
  if(needsUserChange)
   teardown();
 
  injectWidget(user);
 
  lastUserId=user.id;
  lastPath=path;
 }
 
}
 
/* ---------------- TRIGGERS ---------------- */
 
evaluate();
 
window.addEventListener("load", evaluate);
 
window.addEventListener("popstate", evaluate);
 
var pushState = history.pushState;
history.pushState = function () {
 pushState.apply(history, arguments);
 evaluate();
};
 
document.addEventListener("visibilitychange", function(){
 if(!document.hidden) evaluate();
});
 
/* LOGIN DETECTION LOOP */
 
var lastUserCheck=null;
 
setInterval(function(){
 
 if(window.__BUBBLE_DISABLED__) return;
 
 var user=getUser();
 var id=(user && user.id) ? user.id : "";
 
 if(id!==lastUserCheck){
 
  lastUserCheck=id;
 
  evaluate();
 
 }
 
},1000);
 
})();
</script>
""";
 
            return new SnippetResponse(workspaceSlug, snippet);
        }
    }
}
 