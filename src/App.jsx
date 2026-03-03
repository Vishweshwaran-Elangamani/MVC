import { Routes, Route, Navigate } from "react-router-dom";
import { useAuth } from "./context/AuthContext";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Layout from "./pages/Layout";
import Workspaces from "./pages/Workspaces";
import Customize from "./pages/Customize";
import Snippets from "./pages/Snippets";
 
export default function App() {
  const { token } = useAuth();
 
  return (
    <Routes>
      <Route path="/login" element={!token ? <Login /> : <Navigate to="/" />} />
      <Route path="/register" element={!token ? <Register /> : <Navigate to="/" />} />
 
      <Route path="/" element={token ? <Layout /> : <Navigate to="/login" />}>
        <Route index element={<Workspaces />} />
        <Route path="customize" element={<Customize />} />
        <Route path="snippets" element={<Snippets />} />
      </Route>
    </Routes>
  );
}
 