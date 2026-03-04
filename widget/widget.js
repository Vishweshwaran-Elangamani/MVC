(function () {
 
const VERSION = "bubble-widget v3.0";
 
function start(){
 
const u = window.BUBBLE_USER || {};
 
if(!u || !u.workspace) return;
 
const WORKSPACE = u.workspace;
const KEY = u.key || "";
const USER_ID = u.id || "";
const API = "http://localhost:5013";
 
let stopLongPoll=false;
 
/* ---------------- REMOVE WIDGET ---------------- */
 
function removeWidget(){
 
stopLongPoll = true;
 
/* prevent snippet reinjection */
window.__BUBBLE_DISABLED__ = true;
 
const btn=document.getElementById("bubble-btn");
const panel=document.getElementById("bubble-panel");
 
if(btn) btn.remove();
if(panel) panel.remove();
 
console.warn("[Bubble] workspace deleted → widget disabled");
 
}
 
/* ---------------- STYLE ---------------- */
 
const style=document.createElement("style");
 
style.textContent=`
#bubble-btn{
position:fixed;
right:16px;
bottom:16px;
width:48px;
height:48px;
border-radius:50%;
background:#5b8def;
color:#fff;
display:flex;
align-items:center;
justify-content:center;
font-weight:700;
cursor:grab;
z-index:2147483647;
}
 
#bubble-panel{
position:fixed;
width:300px;
max-height:380px;
background:#fff;
border:1px solid #e5e7eb;
border-radius:10px;
box-shadow:0 10px 30px rgba(0,0,0,.08);
overflow:hidden;
display:none;
z-index:2147483647;
}
 
#bubble-header{
padding:8px 12px;
font-weight:600;
border-bottom:1px solid #eee;
}
 
#bubble-list{
height:250px;
overflow:auto;
padding:8px 12px;
}
 
#bubble-input{
display:flex;
gap:6px;
padding:8px 12px;
border-top:1px solid #eee;
}
 
#bubble-input input{
flex:1;
border:1px solid #ddd;
border-radius:6px;
padding:6px 8px;
}
 
#bubble-input button{
padding:6px 10px;
background:#5b8def;
color:#fff;
border:none;
border-radius:6px;
cursor:pointer;
}
`;
 
document.head.appendChild(style);
 
/* ---------------- DOM ---------------- */
 
const btn=document.createElement("div");
btn.id="bubble-btn";
btn.textContent="●";
document.body.appendChild(btn);
 
const panel=document.createElement("div");
panel.id="bubble-panel";
 
panel.innerHTML=`
<div id="bubble-header">Notes • ${WORKSPACE}</div>
<div id="bubble-list"></div>
<div id="bubble-input">
<input id="bubble-text" placeholder="Type a note..." />
<button id="bubble-add">Add</button>
</div>
`;
 
document.body.appendChild(panel);
 
const listEl=panel.querySelector("#bubble-list");
 
/* ---------------- PANEL POSITION ---------------- */
 
function positionPanel(){
 
const rect=btn.getBoundingClientRect();
 
const panelW=300;
const panelH=380;
const gap=8;
 
const vw=window.innerWidth;
const vh=window.innerHeight;
 
let left;
let top;
 
if(rect.right + panelW + gap < vw){
 left = rect.right + gap;
}else{
 left = rect.left - panelW - gap;
}
 
if(rect.top + panelH < vh){
 top = rect.top;
}else{
 top = vh - panelH - 10;
}
 
panel.style.left = left + "px";
panel.style.top = top + "px";
 
}
 
/* ---------------- DRAG ---------------- */
 
let dragging=false;
let offset=[0,0];
 
btn.addEventListener("mousedown",e=>{
 
dragging=true;
 
const r=btn.getBoundingClientRect();
 
offset=[e.clientX-r.left,e.clientY-r.top];
 
btn.style.cursor="grabbing";
 
});
 
document.addEventListener("mouseup",()=>{
 
dragging=false;
btn.style.cursor="grab";
 
});
 
document.addEventListener("mousemove",e=>{
 
if(!dragging) return;
 
const size=48;
const vw=window.innerWidth;
const vh=window.innerHeight;
 
let x=e.clientX-offset[0];
let y=e.clientY-offset[1];
 
x=Math.max(0,Math.min(vw-size,x));
y=Math.max(0,Math.min(vh-size,y));
 
btn.style.left=x+"px";
btn.style.top=y+"px";
 
btn.style.right="unset";
btn.style.bottom="unset";
 
if(panel.style.display==="block"){
 positionPanel();
}
 
});
 
/* ---------------- PANEL TOGGLE ---------------- */
 
btn.onclick=()=>{
 
const open = panel.style.display==="none";
 
panel.style.display=open?"block":"none";
 
if(open) positionPanel();
 
};
 
/* ---------------- UTIL ---------------- */
 
function qs(o){
return new URLSearchParams(o).toString();
}
 
async function httpJson(url,opt={}){
 
const r=await fetch(url,opt);
 
if(r.status===404){
removeWidget();
throw new Error("workspace deleted");
}
 
return r.json();
 
}
 
/* ---------------- NOTES ---------------- */
 
const listNotes=()=>httpJson(`${API}/api/notes?${qs({
workspace:WORKSPACE,
key:KEY,
userId:USER_ID
})}`);
 
const addNote=(content)=>httpJson(`${API}/api/notes`,{
method:"POST",
headers:{ "Content-Type":"application/json" },
body:JSON.stringify({
workspace:WORKSPACE,
key:KEY,
userId:USER_ID,
content
})
});
 
const delNote=(id)=>fetch(`${API}/api/notes/${id}?${qs({
workspace:WORKSPACE,
key:KEY,
userId:USER_ID
})}`,{ method:"DELETE" });
 
/* ---------------- RENDER NOTES ---------------- */
 
async function render(){
 
try{
 
const list=await listNotes();
 
listEl.innerHTML=list.map(n=>`
<div style="display:flex;justify-content:space-between;margin:6px 0">
<div style="flex:1;font-size:12px">${n.content}</div>
<button data-id="${n.id}">x</button>
</div>
`).join("");
 
listEl.querySelectorAll("button").forEach(b=>{
b.onclick=async()=>{
await delNote(b.dataset.id);
render();
};
});
 
}catch{}
 
}
 
/* ---------------- ADD NOTE ---------------- */
 
document.getElementById("bubble-add").onclick=async()=>{
 
const inp=document.getElementById("bubble-text");
 
const val=(inp.value||"").trim();
 
if(!val) return;
 
await addNote(val);
 
inp.value="";
 
render();
 
};
 
/* ---------------- APPEARANCE ---------------- */
 
function applyAppearance(cfg){
 
if(cfg.color) btn.style.background=cfg.color;
 
if(cfg.text) btn.textContent=cfg.text;
 
}
 
let version=0;
 
async function initAppearance(){
 
try{
 
const cfg=await httpJson(`${API}/api/widget/config?${qs({
workspace:WORKSPACE,
key:KEY
})}`);
 
applyAppearance(cfg);
 
version=cfg.version||0;
 
}catch{}
 
}
 
async function longPoll(){
 
while(!stopLongPoll){
 
try{
 
const res=await fetch(`${API}/api/widget/config/long?${qs({
workspace:WORKSPACE,
key:KEY,
since:version
})}`);
 
if(res.status===404){
removeWidget();
return;
}
 
if(res.status===200){
 
const cfg=await res.json();
 
applyAppearance(cfg);
 
version=cfg.version||version;
 
}
 
}catch{
 
await new Promise(r=>setTimeout(r,2000));
 
}
 
}
 
}
 
/* ---------------- INIT ---------------- */
 
initAppearance().then(longPoll);
 
render();
 
}
 
if(document.readyState==="loading"){
document.addEventListener("DOMContentLoaded",start);
}else{
start();
}
 
})();
 