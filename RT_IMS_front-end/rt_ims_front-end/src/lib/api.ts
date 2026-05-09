import axios, { type AxiosResponse } from "axios";
import { getToken } from "./auth";

const baseURL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "https://localhost:7026/api";

const api = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use((config) => {
  const token = getToken();

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

export function getApiErrorMessage(error: unknown) {
  if (axios.isAxiosError(error)) {
    const message = error.response?.data?.message || error.response?.data?.title || error.message;
    return typeof message === "string" ? message : "Unable to connect to the API.";
  }

  return "Unable to connect to the API.";
}

export function extractArray<T>(payload: unknown, fallback: T[] = []) {
  if (Array.isArray(payload)) {
    return payload as T[];
  }

  if (payload && typeof payload === "object") {
    const record = payload as Record<string, unknown>;
    const candidate = record.data ?? record.items ?? record.result ?? record.records;

    if (Array.isArray(candidate)) {
      return candidate as T[];
    }
  }

  return fallback;
}

export function extractObject<T>(payload: unknown, fallback: T) {
  if (payload && typeof payload === "object") {
    const record = payload as Record<string, unknown>;
    const candidate = record.data ?? record.result ?? payload;

    if (candidate && typeof candidate === "object") {
      return candidate as T;
    }
  }

  return fallback;
}

export async function withFallback<T>(
  request: () => Promise<AxiosResponse<unknown>>,
  fallback: T,
  transform?: (payload: unknown) => T,
) {
  try {
    const response = await request();

    return {
      data: transform ? transform(response.data) : (response.data as T),
      error: "",
      fromFallback: false,
    };
  } catch (error) {
    return {
      data: fallback,
      error: getApiErrorMessage(error),
      fromFallback: true,
    };
  }
}

export default api;
