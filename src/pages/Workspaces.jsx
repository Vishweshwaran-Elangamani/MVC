import React, { useEffect, useState } from "react";
import api from "../api/client";

export default function Workspaces({ onLogout }) {
  const [list, setList] = useState([]);
  const [name, setName] = useState("EEPZ");
  const [busy, setBusy] = useState(false);
  const [snippet, setSnippet] = useState("");
  const [slug, setSlug] = useState("");

  async function load() {
    const res = await api.get("/api/workspaces");
    setList(res.data);
  }
  useEffect(() => { load(); }, []);

  async function create() {
    if (!name.trim()) return;
    setBusy(true);
    try {
      const res = await api.post("/api/workspaces", { name });
      await load();
      setSlug(res.data.slug);
      setSnippet(""); // reset snippet view
    } finally { setBusy(false); }
  }

  async function getSnippet(s) {
    setBusy(true);
    try {
      setSlug(s);
      const res = await api.get(`/api/snippet/${s}`);
      setSnippet(res.data.snippet || "");
    } finally { setBusy(false); }
  }

  function copy() {
    navigator.clipboard.writeText(snippet);
    alert("Snippet copied!");
  }

  return (
    <div style={{maxWidth:900, margin:"32px auto", fontFamily:"sans-serif"}}>
      <div style={{display:"flex", justifyContent:"space-between", alignItems:"center"}}>
        <h2>Workspaces</h2>
        <button onClick={onLogout}>Logout</button>
      </div>

      <div style={{display:"flex", gap:8}}>
        <input placeholder="Workspace name" value={name} onChange={e=>setName(e.target.value)} />
        <button disabled={busy} onClick={create}>Create</button>
      </div>

      <div style={{marginTop:16}}>
        <h3>Existing</h3>
        <ul>
          {list.map(w => (
            <li key={w.id} style={{marginBottom:8}}>
              <strong>{w.name}</strong> — <code>{w.slug}</code>{" "}
              <button disabled={busy} onClick={()=>getSnippet(w.slug)}>Get snippet</button>
            </li>
          ))}
        </ul>
      </div>

      {snippet && (
        <div style={{marginTop:16}}>
          <h3>Snippet for <code>{slug}</code></h3>
          <pre style={{whiteSpace:"pre-wrap", background:"#f7f7f8", padding:12, borderRadius:8}}>
{snippet}
          </pre>
          <button onClick={copy}>Copy snippet</button>
        </div>
      )}
    </div>
  );
}