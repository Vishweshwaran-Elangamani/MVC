import { useEffect, useState } from "react";
import api from "../api/axios";
import { useSearch } from "../Context/SearchContext";
import toast, { Toaster } from "react-hot-toast";
 
export default function Workspaces() {
  const [list, setList] = useState([]);
  const [name, setName] = useState("");
  const { query } = useSearch();
 
  const load = async () => {
    try {
      const res = await api.get("/workspaces");
      setList(res.data);
    } catch {
      toast.error("Failed to load workspaces");
    }
  };
 
  useEffect(() => {
    load();
  }, []);
 
  const createWorkspace = async () => {
    if (!name.trim()) {
      return toast.error("Workspace name required");
    }
 
    try {
      await api.post("/workspaces", { name });
      toast.success("Workspace created");
      setName("");
      load();
    } catch {
      toast.error("Workspace already exists");
    }
  };
 
  const filtered = list.filter(ws =>
    ws.name.toLowerCase().includes(query.toLowerCase())
  );
 
  return (
    <>
      <Toaster position="top-right" />
 
      {/* CREATE SECTION */}
      <div className="card">
        <h2>Create Workspace</h2>
 
        <div className="create-row">
          <input
            placeholder="Enter workspace name"
            value={name}
            onChange={(e)=>setName(e.target.value)}
          />
          <button
            className="primary-btn"
            onClick={createWorkspace}
          >
            Create
          </button>
        </div>
      </div>
 
      {/* WORKSPACE GRID */}
      <div className="grid">
        {filtered.map(ws => (
          <div key={ws.slug} className="workspace-card">
            <h3>{ws.name}</h3>
          </div>
        ))}
      </div>
    </>
  );
}

 