import React, { useEffect, useState } from "react";
import Login from "./pages/Login.jsx";
import Workspaces from "./pages/Workspaces.jsx";

export default function App() {
  const [token, setToken] = useState(localStorage.getItem("bubble_admin_token") || "");

  useEffect(() => {
    if (token) localStorage.setItem("bubble_admin_token", token);
    else localStorage.removeItem("bubble_admin_token");
  }, [token]);

  return token ? <Workspaces onLogout={() => setToken("")} /> : <Login onToken={setToken} />;
}