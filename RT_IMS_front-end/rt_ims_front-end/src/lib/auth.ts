import type { AuthUser, LoginResponse } from "@/types/auth";

const TOKEN_KEY = "rt_ims_token";
const USER_KEY = "rt_ims_user";
const AUTH_EVENT = "rt_ims_auth_changed";

const hasStorage = () => typeof window !== "undefined" && Boolean(window.localStorage);

export function getToken() {
  if (!hasStorage()) {
    return null;
  }

  return window.localStorage.getItem(TOKEN_KEY);
}

export function setSession(session: LoginResponse) {
  if (!hasStorage()) {
    return;
  }

  window.localStorage.setItem(TOKEN_KEY, session.token);
  if (session.user) {
    window.localStorage.setItem(USER_KEY, JSON.stringify(session.user));
  }
  notifyAuthChange();
}

export function setDemoSession(username: string) {
  setSession({
    token: `demo-token-${Date.now()}`,
    user: {
      id: "demo-admin",
      name: username || "Demo Admin",
      username: username || "admin",
      role: "Admin",
      status: "Active",
    },
  });
}

export function clearSession() {
  if (!hasStorage()) {
    return;
  }

  window.localStorage.removeItem(TOKEN_KEY);
  window.localStorage.removeItem(USER_KEY);
  notifyAuthChange();
}

export function isAuthenticated() {
  return Boolean(getToken());
}

export function getStoredUser(): AuthUser | null {
  if (!hasStorage()) {
    return null;
  }

  const value = window.localStorage.getItem(USER_KEY);
  if (!value) {
    return null;
  }

  try {
    return JSON.parse(value) as AuthUser;
  } catch {
    return null;
  }
}

export function subscribeAuth(listener: () => void) {
  if (typeof window === "undefined") {
    return () => {};
  }

  window.addEventListener("storage", listener);
  window.addEventListener(AUTH_EVENT, listener);

  return () => {
    window.removeEventListener("storage", listener);
    window.removeEventListener(AUTH_EVENT, listener);
  };
}

export function getAuthSnapshot() {
  return isAuthenticated();
}

export function getServerAuthSnapshot() {
  return false;
}

function notifyAuthChange() {
  if (typeof window !== "undefined") {
    window.dispatchEvent(new Event(AUTH_EVENT));
  }
}
