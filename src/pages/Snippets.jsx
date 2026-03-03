import { useEffect, useState } from "react";
import api from "../api/axios";
import { useSearch } from "../Context/SearchContext";
import toast, { Toaster } from "react-hot-toast";
 
export default function Snippets() {
  const [workspaces, setWorkspaces] = useState([]);
  const [selectedSlug, setSelectedSlug] = useState("");
  const [snippet, setSnippet] = useState("");
  const { query } = useSearch();
 
  useEffect(() => {
    api.get("/workspaces")
      .then(res => setWorkspaces(res.data))
      .catch(() => toast.error("Failed to load workspaces"));
  }, []);
 
  const loadSnippet = async (slug) => {
    try {
      const res = await api.get(`/snippet/${slug}`);
      setSnippet(res.data.snippet);
      setSelectedSlug(slug);
    } catch {
      toast.error("Failed to load snippet");
    }
  };
 
  const copySnippet = () => {
    navigator.clipboard.writeText(snippet);
    toast.success("Snippet copied");
  };
 
  const filtered = workspaces.filter(ws =>
    ws.name.toLowerCase().includes(query.toLowerCase())
  );
 
  return (
    <>
      <Toaster position="top-right" />
 
      <div className="snippet-container">
 
        <div className="snippet-list">
          <h2>Workspaces</h2>
 
          {filtered.map(ws => (
            <div
              key={ws.slug}
              className={`snippet-item ${selectedSlug === ws.slug ? "active-snippet" : ""}`}
              onClick={() => loadSnippet(ws.slug)}
            >
              {ws.name}
            </div>
          ))}
        </div>
 
        <div className="snippet-view">
          <h2>Snippet</h2>
 
          {snippet ? (
            <>
              <textarea
                value={snippet}
                readOnly
                className="snippet-textarea"
              />
              <button
                className="primary-btn"
                onClick={copySnippet}
                style={{ marginTop: "15px" }}
              >
                Copy Snippet
              </button>
            </>
          ) : (
            <p>Select workspace to view snippet</p>
          )}
        </div>
 
      </div>
    </>
  );
}
 