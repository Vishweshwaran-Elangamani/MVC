import { useEffect, useState } from "react";
import api from "../api/axios";
import { useSearch } from "../Context/SearchContext";
import toast, { Toaster } from "react-hot-toast";
 
export default function Customize() {
  const [workspaces, setWorkspaces] = useState([]);
  const [selected, setSelected] = useState(null);
  const { query } = useSearch();
 
  const [appearance, setAppearance] = useState({
    bubbleColor: "#6366f1",
    bubbleText: "●"
  });
 
  useEffect(() => {
    api.get("/workspaces")
      .then(res => setWorkspaces(res.data))
      .catch(() => toast.error("Failed to load workspaces"));
  }, []);
 
  const loadAppearance = async (ws) => {
    setSelected(ws);
 
    try {
      const res = await api.get(`/workspaces/${ws.id}/appearance`);
 
      setAppearance({
        bubbleColor: res.data.bubbleColor || "#6366f1",
        bubbleText: res.data.bubbleText || "●"
      });
    } catch {
      toast.error("Failed to load appearance");
    }
  };
 
  const saveChanges = async () => {
    if (!selected) return toast.error("Select workspace first");
 
    if (appearance.bubbleText.length > 5)
      return toast.error("Maximum 5 characters allowed");
 
    try {
      await api.put(
        `/workspaces/${selected.id}/appearance`,
        appearance
      );
      toast.success("Bubble updated");
    } catch {
      toast.error("Update failed");
    }
  };
 
  const filtered = workspaces.filter(ws =>
    ws.name.toLowerCase().includes(query.toLowerCase())
  );
 
  return (
    <>
      <Toaster position="top-right" />
 
      <div className="customize-wrapper">
 
        {/* LEFT SIDE - WORKSPACE LIST */}
        <div className="workspace-side">
          <h2>Workspaces</h2>
 
          {filtered.map(ws => (
            <div
              key={ws.id}
              className={`workspace-item ${selected?.id === ws.id ? "active-item" : ""}`}
              onClick={() => loadAppearance(ws)}
            >
              {ws.name}
            </div>
          ))}
        </div>
 
        {/* RIGHT SIDE - SPLIT AREA */}
        <div className="customize-split">
 
          {/* CONTROL PANEL */}
          <div className="control-panel">
            {selected ? (
              <>
                <h2>Customize - {selected.name}</h2>
 
                <div className="field">
                  <label>Bubble Color</label>
                  <input
                    type="color"
                    className="color-picker"
                    value={appearance.bubbleColor}
                    onChange={(e) =>
                      setAppearance({
                        ...appearance,
                        bubbleColor: e.target.value
                      })
                    }
                  />
                </div>
 
                <div className="field">
                  <label>Bubble Text</label>
                  <input
                    value={appearance.bubbleText}
                    maxLength={5}
                    onChange={(e) =>
                      setAppearance({
                        ...appearance,
                        bubbleText: e.target.value
                      })
                    }
                  />
                </div>
 
                <button
                  className="primary-btn"
                  onClick={saveChanges}
                >
                  Save Changes
                </button>
              </>
            ) : (
              <p>Select workspace to customize</p>
            )}
          </div>
 
          {/* PREVIEW PANEL */}
          <div className="preview-panel">
            {selected ? (
              <>
                <div
                  className="custom-bubble"
                  style={{ background: appearance.bubbleColor }}
                >
                  {appearance.bubbleText}
                </div>
                <p className="preview-text">Live Preview</p>
              </>
            ) : (
              <div className="empty-preview">
                <p>Select a workspace to see preview</p>
              </div>
            )}
          </div>
 
        </div>
 
      </div>
    </>
  );
}
 