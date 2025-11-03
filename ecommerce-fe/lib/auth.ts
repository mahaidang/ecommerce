import { jwtDecode } from "jwt-decode";

// Định nghĩa kiểu payload của JWT (tùy backend)
interface JwtPayload {
  sub?: string;         // userId (theo chuẩn JWT)
  userId?: string;      // userId (theo BE trả về)
  email?: string;
  username?: string;
  exp?: number;
  [key: string]: any;
}

/**
 * Lấy access_token từ localStorage (theo response mới)
 */
export function getAccessToken(): string | null {
  if (typeof window === "undefined") return null;
  // BE trả về key là "token"
  return localStorage.getItem("token");
}

/**
 * Lấy userId từ localStorage hoặc từ token (ưu tiên localStorage)
 */
export function getUserIdFromToken(): string | null {
  if (typeof window === "undefined") return null;
  // BE trả về userId riêng ngoài token
  const userId = localStorage.getItem("userId") || localStorage.getItem("user_id");
  if (userId) return userId;
  const token = getAccessToken();
  if (!token) return null;
  try {
    const decoded = jwtDecode<JwtPayload>(token);
    return decoded.sub || decoded.userId || null;
  } catch (err) {
    console.error("Invalid token", err);
    return null;
  }
}

/**
 * Lấy toàn bộ user info từ localStorage hoặc từ token
 */
export function getUserInfoFromToken(): JwtPayload | null {
  if (typeof window === "undefined") return null;
  const token = getAccessToken();
  if (!token) return null;
  try {
    return jwtDecode<JwtPayload>(token);
  } catch {
    return null;
  }
}
