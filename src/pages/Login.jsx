import { useState } from "react";
import api from "../api/axios";
import { useAuth } from "../Context/AuthContext";
import { useNavigate } from "react-router-dom";
import toast, { Toaster } from "react-hot-toast";
 
export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const { login } = useAuth();
  const navigate = useNavigate();
 
  const handle = async () => {
    try {
      const res = await api.post("/auth/login", { email, password });
      login(res.data);
      navigate("/");
    } catch {
      toast.error("Invalid credentials");
    }
  };
 
  return (
    <div className="login-wrapper">
      <Toaster position="top-right" />
 
      <div className="login-card">
        <h1>Bubble Admin</h1>
 
        <input
          className="login-input"
          placeholder="Email"
          onChange={(e) => setEmail(e.target.value)}
        />
 
        <input
          className="login-input"
          type="password"
          placeholder="Password"
          onChange={(e) => setPassword(e.target.value)}
        />
 
        <div className="login-buttons">
          <button className="login-primary" onClick={handle}>
            Login
          </button>
 
          <button
            className="login-secondary"
            onClick={() => navigate("/register")}
          >
            Register
          </button>
        </div>
      </div>
    </div>
  );
}
 