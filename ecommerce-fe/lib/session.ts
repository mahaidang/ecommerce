import { v4 as uuidv4 } from "uuid";

export function getOrCreateSessionId(): string {
  const key = "guest_session_id";
  let sessionId = localStorage.getItem(key);
  if (!sessionId) {
    sessionId = uuidv4();
    localStorage.setItem(key, sessionId);
  }
  return sessionId;
}
