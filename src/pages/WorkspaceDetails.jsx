import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import axios from "../api/axios";
import toast from "react-hot-toast";
 
export default function WorkspaceDetails() {
  const { slug } = useParams();
  const [id, setId] = useState("");
  const [snippet, setSnippet] = useState("");
  const [appearance, setAppearance] = useState({ bubbleColor:"#5b8def", bubbleText:"●" });
 
  useEffect(()=>{
    axios.get(`/workspaces/${slug}`).then(r=>setId(r.data.id));
    axios.get(`/snippet/${slug}`).then(r=>setSnippet(r.data.snippet));
  },[slug]);
 
  useEffect(()=>{
    if(id){
      axios.get(`/workspaces/${id}/appearance`).then(r=>setAppearance(r.data));
    }
  },[id]);
 
  const save = async ()=>{
    await axios.put(`/workspaces/${id}/appearance`, appearance);
    toast.success("Updated");
  };
 
  return (
    <>
      <div className="card">
        <h2>Customize Bubble</h2>
 
        <div className="preview">
          <div
            className="bubble-preview"
            style={{ background: appearance.bubbleColor }}
          >
            {appearance.bubbleText}
          </div>
        </div>
 
        <input type="color"
          value={appearance.bubbleColor}
          onChange={(e)=>setAppearance({...appearance,bubbleColor:e.target.value})}
        />
 
        <input
          value={appearance.bubbleText}
          onChange={(e)=>setAppearance({...appearance,bubbleText:e.target.value})}
        />
 
        <button onClick={save}>Save</button>
      </div>
 
      <div className="card">
        <h2>Embed Snippet</h2>
        <textarea value={snippet} readOnly />
        <button onClick={()=>navigator.clipboard.writeText(snippet)}>Copy Snippet</button>
      </div>
    </>
  );
}
 