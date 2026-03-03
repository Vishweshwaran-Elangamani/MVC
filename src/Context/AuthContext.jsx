import { createContext, useContext, useState } from "react";
 
const AuthContext = createContext();
 
export function AuthProvider({ children }) {
  const [token, setToken] = useState(localStorage.getItem("token"));
  const [email, setEmail] = useState(localStorage.getItem("email"));
 
  const login = (data) => {
    localStorage.setItem("token", data.token);
    localStorage.setItem("email", data.adminEmail);
    setToken(data.token);
    setEmail(data.adminEmail);
  };
 
  const logout = () => {
    localStorage.clear();
    setToken(null);
    setEmail(null);
  };
 
  return (
    <AuthContext.Provider value={{ token, email, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
 
export const useAuth = () => useContext(AuthContext);
 