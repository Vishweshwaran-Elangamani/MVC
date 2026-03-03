import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE,
  headers: { "Content-Type": "application/json" }
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("bubble_admin_token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default api;

/** === Appearance helpers (align with your new backend) ===
 * GET  /api/workspaces/{workspaceId}/appearance  -> { color: string, text: string }
 * PUT  /api/workspaces/{workspaceId}/appearance  -> { Color, Text }
 */
export const getAppearance = async (workspaceId: string) => {
  const res = await api.get(`/api/workspaces/${workspaceId}/appearance`);
  return res.data as { color: string; text: string };
};

export const updateAppearance = async (
  workspaceId: string,
  payload: { Color: string; Text: string }
) => {
  await api.put(`/api/workspaces/${workspaceId}/appearance`, payload);
};