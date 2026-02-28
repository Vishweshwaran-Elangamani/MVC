(function () {
  const __WIDGET_VERSION__ = "bubble-widget v1.2 (panel follows button; clears right/bottom)";

  function start() {
    console.log("[Bubble] starting", __WIDGET_VERSION__);

    // ---- Read user injected by the snippet ----
    const u = window.BUBBLE_USER || {};
    if (!u || !u.id) {
      console.warn("[Bubble] Missing window.BUBBLE_USER.id");
      return;
    }

    const WORKSPACE = u.workspace || "default";
    const API_BASE = "http://localhost:5013"; // <-- set to your API URL

    // ---- Styles (panel uses left/top; not right/bottom) ----
    const style = document.createElement("style");
    style.textContent = `
      #bubble-btn{
        position:fixed; right:16px; bottom:16px; width:48px; height:48px; border-radius:50%;
        background:#5b8def; color:#fff; display:flex; align-items:center; justify-content:center;
        font-weight:700; cursor:grab; z-index:2147483647;
      }
      #bubble-panel{
        position:fixed; left:0; top:0; width:300px; max-height:380px; background:#fff;
        border:1px solid #e5e7eb; border-radius:10px; box-shadow:0 10px 30px rgba(0,0,0,.08);
        overflow:hidden; display:none; z-index:2147483647;
      }
      #bubble-header{ padding:8px 12px; font-weight:600; border-bottom:1px solid #eee; }
      #bubble-list{ height:250px; overflow:auto; padding:8px 12px; }
      #bubble-input{ display:flex; gap:6px; padding:8px 12px; border-top:1px solid #eee; }
      #bubble-input input{ flex:1; border:1px solid #ddd; border-radius:6px; padding:6px 8px; }
      #bubble-input button{ padding:6px 10px; background:#5b8def; color:#fff; border:none; border-radius:6px; }
      #bubble-error{ color:#b91c1c; padding:4px 12px; display:none; }
    `;
    document.head.appendChild(style);

    // ---- Button & panel DOM ----
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
      </div>
    `;
    document.body.appendChild(panel);

    const elErr  = panel.querySelector("#bubble-error");
    const elList = panel.querySelector("#bubble-list");
    const BTN_SIZE  = 48;   // keep in sync with CSS
    const PANEL_W   = 300;  // keep in sync with CSS
    const PANEL_H   = 380;  // keep in sync with CSS max-height
    const PANEL_GAP = 8;

    // ---- Position state & helpers ----
    let btnPos = { left: null, top: null }; // last known button position (px)

    function placePanelNearButton() {
      if (!panel || panel.style.display === "none") return;

      const vw = window.innerWidth, vh = window.innerHeight;
      let left = btnPos.left;
      let top  = btnPos.top;

      // If no explicit left/top yet (first open), derive from DOM rect
      if (left == null || top == null) {
        const rect = btn.getBoundingClientRect();
        left = rect.left;
        top  = rect.top;
      }

      // Decide side (left/right) and vertical flip (up/down) to keep panel on-screen
      const openLeft = (left + BTN_SIZE + PANEL_GAP + PANEL_W > vw);
      const openUp   = (top + PANEL_H > vh);

      const panelLeft = openLeft
        ? Math.max(0, left - PANEL_GAP - PANEL_W)
        : Math.min(vw - PANEL_W, left + BTN_SIZE + PANEL_GAP);

      const panelTop = openUp
        ? Math.max(0, top + BTN_SIZE - PANEL_H)
        : Math.min(vh - PANEL_H, top);

      // Apply left/top and explicitly clear right/bottom to defeat any old CSS
      panel.style.left   = `${Math.round(panelLeft)}px`;
      panel.style.top    = `${Math.round(panelTop)}px`;
      panel.style.right  = "unset";
      panel.style.bottom = "unset";
    }

    window.addEventListener("resize", placePanelNearButton);

    // ---- Drag behavior (left/top positioning, panel follows) ----
    let dragging = false, offset = [0, 0];

    // Initial button position expressed via left/top (not right/bottom)
    (function setInitialBtnPosition() {
      const vw = window.innerWidth, vh = window.innerHeight;
      btnPos.left = vw - BTN_SIZE - 16;
      btnPos.top  = vh - BTN_SIZE - 16;
      btn.style.left   = `${btnPos.left}px`;
      btn.style.top    = `${btnPos.top}px`;
      btn.style.right  = "unset";
      btn.style.bottom = "unset";
    })();

    btn.addEventListener("mousedown", e => {
      dragging = true;
      const rect = btn.getBoundingClientRect();
      offset = [e.clientX - rect.left, e.clientY - rect.top];
      btn.style.cursor = "grabbing";
    });

    document.addEventListener("mouseup", () => {
      if (!dragging) return;
      dragging = false;
      btn.style.cursor = "grab";
      const vw = window.innerWidth, vh = window.innerHeight;
      btnPos.left = Math.min(vw - BTN_SIZE, Math.max(0, btn.offsetLeft));
      btnPos.top  = Math.min(vh - BTN_SIZE, Math.max(0, btn.offsetTop));
      placePanelNearButton();
    });

    document.addEventListener("mousemove", e => {
      if (!dragging) return;

      const vw = window.innerWidth, vh = window.innerHeight;
      let x = e.clientX - offset[0];
      let y = e.clientY - offset[1];

      x = Math.min(vw - BTN_SIZE, Math.max(0, x));
      y = Math.min(vh - BTN_SIZE, Math.max(0, y));

      btn.style.left   = `${x}px`;
      btn.style.top    = `${y}px`;
      btn.style.right  = "unset";
      btn.style.bottom = "unset";

      btnPos.left = x;
      btnPos.top  = y;

      placePanelNearButton();
    });

    // ---- Toggle panel near current button position ----
    btn.addEventListener("click", () => {
      const willOpen = panel.style.display === "none";
      panel.style.display = willOpen ? "block" : "none";
      if (willOpen) {
        if (btnPos.left == null || btnPos.top == null) {
          const rect = btn.getBoundingClientRect();
          btnPos.left = rect.left;
          btnPos.top  = rect.top;
        }
        // Clear right/bottom just in case some CSS injected them
        panel.style.right  = "unset";
        panel.style.bottom = "unset";
        placePanelNearButton();
      }
    });

    // ---- Utilities ----
    function showError(msg) {
      elErr.style.display = "block";
      elErr.textContent = msg;
      setTimeout(() => { elErr.style.display = "none"; }, 4000);
    }

    function escapeHtml(s){
      return String(s || "").replace(/[&<>\"']/g, c => ({
        "&":"&", "<":"<", ">":">", "\"":"\"", "'":"'"
      }[c]));
    }

    async function httpJson(url, options = {}, retries = 1) {
      try {
        const res = await fetch(url, options);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        return await res.json();
      } catch (err) {
        if (retries > 0) return httpJson(url, options, retries - 1);
        throw err;
      }
    }

    function qs(obj) { return new URLSearchParams(obj).toString(); }

    // ---- API calls ----
    const listNotes = () =>
      httpJson(`${API_BASE}/api/notes?${qs({ workspace: WORKSPACE, userId: String(u.id) })}`);

    const addNote = (content) =>
      httpJson(`${API_BASE}/api/notes`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ workspace: WORKSPACE, userId: String(u.id), content })
      });

    const delNote = (id) =>
      fetch(`${API_BASE}/api/notes/${id}?${qs({ workspace: WORKSPACE, userId: String(u.id) })}`,
        { method: "DELETE" });

    // ---- Render ----
    async function render() {
      try {
        const list = await listNotes();
        elList.innerHTML = list.map(n => `
          <div style="display:flex;justify-content:space-between;gap:8px;margin:6px 0;">
            <div style="flex:1;font-size:12px;color:#333;">${escapeHtml(n.content)}</div>
            <button data-id="${n.id}" style="border:none;background:#f3f4f6;border-radius:6px;padding:4px 6px;">x</button>
          </div>
        `).join("");

        elList.querySelectorAll("button[data-id]").forEach(b => b.onclick = async () => {
          await delNote(b.dataset.id);
          render();
        });
      } catch (e) {
        showError("Failed to load notes");
      }
    }

    // ---- Add handler ----
    document.getElementById("bubble-add").onclick = async () => {
      const inp = document.getElementById("bubble-text");
      const val = (inp.value || "").trim();
      if (!val) return;
      try {
        await addNote(val);
        inp.value = "";
        render();
      } catch {
        showError("Failed to add note");
      }
    };

    // Initial render
    render();
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", start);
  } else {
    start();
  }
  
})();