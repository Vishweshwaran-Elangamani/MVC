import { useState } from "react";
import api from "../api/axios";
import { useNavigate } from "react-router-dom";
import toast, { Toaster } from "react-hot-toast";
 
export default function Register() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
 
  const handle = async () => {
    try {
      await api.post("/auth/register", { email, password });
      toast.success("Account created successfully");
      navigate("/login");
    } catch {
      toast.error("Email already exists");
    }
  };
 
  return (
    <div className="register-wrapper">
      <Toaster position="top-right" />
 
      <div className="register-card">
        <h1>Create Account</h1>
 
        <input
          className="register-input"
          placeholder="Email"
          onChange={(e) => setEmail(e.target.value)}
        />
 
        <input
          className="register-input"
          type="password"
          placeholder="Password"
          onChange={(e) => setPassword(e.target.value)}
        />
 
        <button className="register-primary" onClick={handle}>
          Register
        </button>
      </div>
    </div>
  );
}
