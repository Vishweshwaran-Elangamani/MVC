// D:\bubble-admin\src\components\AppearancePanel.jsx
import React, { useEffect, useState } from "react";
import { getAppearance, updateAppearance } from "../../api/client";

export default function AppearancePanel({ workspace }) {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving]   = useState(false);
  const [err, setErr]         = useState("");
  const [ok, setOk]           = useState(false);

  const [color, setColor] = useState("#5b8def");
  const [text,  setText]  = useState("●");

  useEffect(() => {
    let cancel = false;
    (async () => {
      setLoading(true);
      setErr("");
      setOk(false);
      try {
        const a = await getAppearance(workspace.id);
        if (!cancel) {
          setColor(a.color || "#5b8def");
          setText(a.text || "●");
        }
      } catch {
        if (!cancel) setErr("Failed to load appearance");
      } finally {
        if (!cancel) setLoading(false);
      }
    })();
    return () => { cancel = true; };
  }, [workspace.id]);

  async function save() {
    setSaving(true);
    setErr("");
    setOk(false);
    try {
      await updateAppearance(workspace.id, { Color: color, Text: text });
      setOk(true);
      setTimeout(() => setOk(false), 2000);
    } catch {
      setErr("Failed to save appearance");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div style={{border:"1px solid #e5e7eb", borderRadius:8, padding:12, marginTop:8}}>
      <div style={{display:"flex", gap:16, alignItems:"center", flexWrap:"wrap"}}>
        <div>
          <label style={{fontSize:12, color:"#444"}}>Bubble color</label><br/>
          <input
            type="color"
            value={color}
            onChange={e => setColor(e.target.value)}
            style={{width:48, height:32, border:"none", background:"transparent", cursor:"pointer"}}
            disabled={loading || saving}
          />
          <input
            value={color}
            onChange={e => setColor(e.target.value)}
            style={{marginLeft:8, width:120}}
            disabled={loading || saving}
          />
        </div>

        <div>
          <label style={{fontSize:12, color:"#444"}}>Bubble text</label><br/>
          <input
            value={text}
            onChange={e => setText(e.target.value)}
            maxLength={3}
            style={{width:80}}
            disabled={loading || saving}
          />
        </div>

        <div style={{marginLeft:"auto", display:"flex", gap:8}}>
          <button onClick={save} disabled={loading || saving}>
            {saving ? "Saving..." : "Save"}
          </button>
        </div>
      </div>

      {/* Preview */}
      <div style={{marginTop:12}}>
        <span style={{fontSize:12, color:"#555"}}>Preview:</span>
        <div
          style={{
            display:"inline-flex", alignItems:"center", justifyContent:"center",
            width:48, height:48, borderRadius:"50%", marginLeft:8,
            background: color, color:"#fff", fontWeight:700
          }}
          title="Bubble"
        >
          {text || "●"}
        </div>
      </div>

      {err && <div style={{color:"#b91c1c", marginTop:8}}>{err}</div>}
      {ok  && <div style={{color:"#16a34a", marginTop:8}}>Saved. Widgets will update in a few seconds.</div>}
    </div>
  );
}