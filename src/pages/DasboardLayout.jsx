import { Outlet, NavLink } from "react-router-dom";
import { useAuth } from "../Context/AuthContext";
 
export default function DashboardLayout() {
  const { email, logout } = useAuth();
 
  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="logo">Bubble</div>
        <NavLink to="/" end>Workspaces</NavLink>
      </aside>
 
      <div className="main">
        <header className="topbar">
          <span>Welcome, {email}</span>
          <button onClick={logout}>Logout</button>
        </header>
        <div className="content">
          <Outlet />
        </div>
      </div>
    </div>
  );
}
 