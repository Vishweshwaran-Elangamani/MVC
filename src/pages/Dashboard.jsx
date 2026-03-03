import { useEffect, useState } from "react";
import axios from "../api/axios";
import { useAuth } from "../Context/AuthContext";
import { Link } from "react-router-dom";
import toast, { Toaster } from "react-hot-toast";
 
export default function Dashboard() {
  const { email, logout } = useAuth();
  const [workspaces, setWorkspaces] = useState([]);
  const [name, setName] = useState("");
 
  const load = async () => {
    const res = await axios.get("/workspaces");
    setWorkspaces(res.data);
  };
 
  useEffect(() => { load(); }, []);
 
  const createWorkspace = async () => {
    try {
      await axios.post("/workspaces", { name });
      toast.success("Workspace created");
      setName("");
      load();
    } catch (e) {
      if (e.response?.status === 409) toast.error("Workspace already exists");
    }
  };
 
  return (
    <div>
      <Toaster />
      <div className="navbar">
        <h3>Bubble App</h3>
        <div>
          Welcome, {email}
          <button onClick={logout}>Logout</button>
        </div>
      </div>
 
      <div className="container">
        <div className="card">
          <h3>Create Workspace</h3>
          <input placeholder="Workspace Name" value={name} onChange={(e) => setName(e.target.value)} />
          <button onClick={createWorkspace}>Create</button>
        </div>
 
        <div className="card">
          <h3>Your Workspaces</h3>
          {workspaces.map((w) => (
            <Link key={w.slug} to={`/workspace/${w.slug}`} className="workspace">
              {w.name}
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}
 