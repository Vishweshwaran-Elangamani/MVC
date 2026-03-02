import React, { useEffect, useState } from "react";
import api from "../api/client";

export default function Workspaces({ onLogout }) {
  const [list, setList] = useState([]);
  const [name, setName] = useState("EEPZ");
  const [busy, setBusy] = useState(false);
  const [snippet, setSnippet] = useState("");
  const [slug, setSlug] = useState("");
  const [err, setErr] = useState("");

  async function load() {
    const res = await api.get("/api/workspaces");
    setList(res.data);
  }

  useEffect(() => {
    load();
    // Make API base readable by the static viewer page
    localStorage.setItem("VITE_API_BASE", import.meta.env.VITE_API_BASE);
  }, []);

  async function create() {
    if (!name.trim()) return;
    setBusy(true);
    setErr("");
    try {
      const res = await api.post("/api/workspaces", { name });
      await load();
      setSlug(res.data.slug);
      setSnippet("");
    } catch {
      setErr("Failed to create workspace (maybe name already exists).");
    } finally {
      setBusy(false);
    }
  }

  async function getSnippet(s) {
    setBusy(true);
    setErr("");
    try {
      setSlug(s);
      const res = await api.get(`/api/snippet/${s}`);
      setSnippet(res.data.snippet || "");
    } catch {
      setErr("Failed to fetch snippet (check auth/token).");
    } finally {
      setBusy(false);
    }
  }

  function copy() {
    navigator.clipboard.writeText(snippet);
    alert("Snippet copied!");
  }

  function downloadHtml() {
    const blob = new Blob([snippet], { type: "text/plain;charset=utf-8" });
    const a = document.createElement("a");
    a.href = URL.createObjectURL(blob);
    a.download = `bubble-snippet-${slug}.html`;
    document.body.appendChild(a);
    a.click();
    a.remove();
  }

  return (
    <div style={{maxWidth:900, margin:"32px auto", fontFamily:"system-ui, -apple-system, Segoe UI, Roboto, sans-serif"}}>
      <div style={{display:"flex", justifyContent:"space-between", alignItems:"center"}}>
        <h2>Workspaces</h2>
        <button onClick={onLogout}>Logout</button>
      </div>

      <div style={{display:"flex", gap:8}}>
        <input
          placeholder="Workspace name"
          value={name}
          onChange={(e)=>setName(e.target.value)}
        />
        <button disabled={busy} onClick={create}>Create</button>
      </div>

      {err && <div style={{color:"crimson", marginTop:8}}>{err}</div>}

      <div style={{marginTop:16}}>
        <h3>Existing</h3>
        <ul>
          {list.map(w => (
            <li key={w.id} style={{marginBottom:8}}>
              <strong>{w.name}</strong> — <code>{w.slug}</code>{" "}
              <button disabled={busy} onClick={() => getSnippet(w.slug)}>Get snippet</button>{" "}
              {/* Open authorized viewer (static HTML) in a new tab */}
              <a
                href={`/snippet-viewer.html?slug=${encodeURIComponent(w.slug)}`}
                target="_blank"
                rel="noreferrer"
              >
                Open viewer
              </a>
            </li>
          ))}
        </ul>
      </div>

      {snippet && (
        <div style={{marginTop:16}}>
          <h3>Snippet for <code>{slug}</code></h3>

          {/* Show the snippet exactly as it will be pasted */}
          <textarea
            readOnly
            value={snippet}
            style={{
              width: "100%",
              height: 280,
              fontFamily: "ui-monospace, SFMono-Regular, Menlo, Consolas, monospace",
              fontSize: 13,
              lineHeight: 1.35,
              background: "#0b1020",
              color: "#e7eaf6",
              padding: 12,
              borderRadius: 8,
              border: "1px solid #222"
            }}
          />

          <div style={{display:"flex", gap:8, marginTop:8}}>
            <button onClick={copy}>Copy snippet</button>
            <button onClick={downloadHtml}>Download .html</button>
          </div>
        </div>
      )}
    </div>
  );
}