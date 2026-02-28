(function () {
    function start() {
      const u = window.BUBBLE_USER || {};
      if (!u || !u.id) {
        console.warn("[Bubble] Missing window.BUBBLE_USER.id");
        return;
      }
        const WORKSPACE = u.workspace || "default";
        const API_BASE = "http://localhost:5013"; // or https if you run https
     
      // Styles
      const style = document.createElement("style");
      style.textContent = `
        #bubble-btn{position:fixed;right:16px;bottom:16px;width:48px;height:48px;border-radius:50%;
          background:#5b8def;color:#fff;display:flex;align-items:center;justify-content:center;
          font-weight:700;cursor:grab;z-index:2147483647;}
        #bubble-panel{position:fixed;right:72px;bottom:16px;width:300px;max-height:380px;background:#fff;
          border:1px solid #e5e7eb;border-radius:10px;box-shadow:0 10px 30px rgba(0,0,0,.08);
          overflow:hidden;display:none;z-index:2147483647;}
        #bubble-header{padding:8px 12px;font-weight:600;border-bottom:1px solid #eee}
        #bubble-list{height:250px;overflow:auto;padding:8px 12px}
        #bubble-input{display:flex;gap:6px;padding:8px 12px;border-top:1px solid #eee}
        #bubble-input input{flex:1;border:1px solid #ddd;border-radius:6px;padding:6px 8px}
        #bubble-input button{padding:6px 10px;background:#5b8def;color:#fff;border:none;border-radius:6px}
        #bubble-error{color:#b91c1c;padding:4px 12px;display:none}
      `;
      document.head.appendChild(style);
  
      // Elements
      const btn = document.createElement("div");
      btn.id = "bubble-btn";
      btn.textContent = "●";
      document.body.appendChild(btn);
  
      const panel = document.createElement("div");
      panel.id = "bubble-panel";
      panel.innerHTML = `
        <div id="bubble-header">My Notes</div>
        <div id="bubble-error"></div>
        <div id="bubble-list"></div>
        <div id="bubble-input">
          <input id="bubble-text" placeholder="Type a note..." />
          <button id="bubble-add">Add</button>
        </div>`;
      document.body.appendChild(panel);
  
      const elErr  = panel.querySelector("#bubble-error");
      const elList = panel.querySelector("#bubble-list");
  
      // Toggle
      btn.addEventListener("click", () => {
        panel.style.display = panel.style.display === "none" ? "block" : "none";
      });
  
      // Drag
      let down=false, off=[0,0];
      btn.addEventListener("mousedown", e => { down=true; off=[btn.offsetLeft-e.clientX, btn.offsetTop-e.clientY]; btn.style.cursor="grabbing"; });
      document.addEventListener("mouseup", () => { down=false; btn.style.cursor="grab"; });
      document.addEventListener("mousemove", e => {
        if (!down) return;
        const x=e.clientX+off[0], y=e.clientY+off[1];
        btn.style.left=`${Math.min(window.innerWidth-48, Math.max(0,x))}px`;
        btn.style.top =`${Math.min(window.innerHeight-48, Math.max(0,y))}px`;
        btn.style.right="unset"; btn.style.bottom="unset";
      });
  
      // Helpers
      function showError(msg) {
        elErr.style.display = "block";
        elErr.textContent = msg;
        setTimeout(()=>{ elErr.style.display = "none"; }, 4000);
      }
      function escapeHtml(s){ return String(s||"").replace(/[&<>"']/g, c=>({ "&":"&amp;","<":"&lt;",">":"&gt;","\"":"&quot;","'":"&#39;" }[c])); }
  
      async function httpJson(url, options={}, retries=1) {
        try {
          const res = await fetch(url, options);
          if (!res.ok) throw new Error(`HTTP ${res.status}`);
          return await res.json();
        } catch (err) {
          if (retries > 0) return httpJson(url, options, retries-1);
          throw err;
        }
      }
  
      // API
      function qs(obj){ return new URLSearchParams(obj).toString(); }
      const listNotes = () =>
        httpJson(`${API_BASE}/api/notes?${qs({ workspace:WORKSPACE, userId:String(u.id) })}`);
      const addNote = (content) =>
        httpJson(`${API_BASE}/api/notes`, {
          method:"POST",
          headers:{ "Content-Type":"application/json" },
          body: JSON.stringify({ workspace:WORKSPACE, userId:String(u.id), content })
        });
      const delNote = (id) =>
        fetch(`${API_BASE}/api/notes/${id}?${qs({ workspace:WORKSPACE, userId:String(u.id) })}`, { method:"DELETE" });
  
      // UI
      async function render() {
        try {
          const list = await listNotes();
          elList.innerHTML = list.map(n => `
            <div style="display:flex;justify-content:space-between;gap:8px;margin:6px 0;">
              <div style="flex:1;font-size:12px;color:#333;">${escapeHtml(n.content)}</div>
              <button data-id="${n.id}" style="border:none;background:#f3f4f6;border-radius:6px;padding:4px 6px;">x</button>
            </div>`).join("");
          elList.querySelectorAll("button[data-id]").forEach(b => b.onclick = async () => {
            await delNote(b.dataset.id);
            render();
          });
        } catch (e) {
          showError("Failed to load notes");
        }
      }
  
      document.getElementById("bubble-add").onclick = async () => {
        const inp = document.getElementById("bubble-text");
        const val = (inp.value||"").trim();
        if (!val) return;
        try {
          await addNote(val);
          inp.value = "";
          render();
        } catch {
          showError("Failed to add note");
        }
      };
  
      render();
    }
  
    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", start);
    else start();
  })();