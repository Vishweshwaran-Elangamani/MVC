import React, { useState } from "react";
import api from "../api/client";

export default function Login({ onToken }) {
  const [email, setEmail] = useState("admin@eepz.com");
  const [password, setPassword] = useState("P@ssw0rd!");
  const [busy, setBusy] = useState(false);
  const [err, setErr] = useState("");

  async function submit(e) {
    e.preventDefault();
    setBusy(true); setErr("");
    try {
      const res = await api.post("/api/auth/login", { email, password });
      onToken(res.data.token);
    } catch (ex) {
      setErr("Login failed. Try register in Swagger first.");
    } finally { setBusy(false); }
  }

  return (
    <div style={{maxWidth:420, margin:"64px auto", fontFamily:"sans-serif"}}>
      <h2>Bubble Admin – Login</h2>
      <form onSubmit={submit}>
        <div>
          <label>Email</label><br/>
          <input value={email} onChange={e=>setEmail(e.target.value)} style={{width:"100%"}} />
        </div>
        <div style={{marginTop:8}}>
          <label>Password</label><br/>
          <input type="password" value={password} onChange={e=>setPassword(e.target.value)} style={{width:"100%"}} />
        </div>
        {err && <div style={{color:"crimson", marginTop:8}}>{err}</div>}
        <button disabled={busy} style={{marginTop:12}}>{busy?"..." : "Login"}</button>
      </form>
    </div>
  );
}