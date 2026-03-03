import { Outlet, NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../Context/AuthContext";
import { useSearch } from "../Context/SearchContext";
import { Search } from "lucide-react";
import { useState, useEffect } from "react";
 
export default function Layout() {
  const { email, logout } = useAuth();
  const { setQuery } = useSearch();
  const navigate = useNavigate();
 
  const [input, setInput] = useState("");
 
  // 🔹 Auto reset when empty
  useEffect(() => {
    if (input.trim() === "") {
      setQuery("");
    }
  }, [input, setQuery]);
 
  const handleSearch = () => {
    setQuery(input);
  };
 
  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="logo">Bubble</div>
        <NavLink to="/" end>Workspaces</NavLink>
        <NavLink to="/customize">Customize</NavLink>
        <NavLink to="/snippets">Snippets</NavLink>
      </aside>
 
      <div className="main">
        <header className="header">
 
          <div className="welcome">
            Welcome, {email}
          </div>
 
          <div className="header-right">
            <div className="search-box">
              <input
                placeholder="Search..."
                value={input}
                onChange={(e)=>setInput(e.target.value)}
                onKeyDown={(e)=>{
                  if(e.key === "Enter") handleSearch();
                }}
              />
              <button onClick={handleSearch}>
                <Search size={18} />
              </button>
            </div>
 
            <button
              className="danger-btn"
              onClick={() => {
                logout();
                navigate("/login");
              }}
            >
              Logout
            </button>
          </div>
 
        </header>
 
        <div className="page">
          <Outlet />
        </div>
      </div>
    </div>
  );
}
 