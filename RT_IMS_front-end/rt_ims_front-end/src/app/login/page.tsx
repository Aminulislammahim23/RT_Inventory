"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import api, { getApiErrorMessage } from "@/lib/api";
import { normalizeAuthResponse } from "@/lib/api-contract";
import { setDemoSession, setSession } from "@/lib/auth";
import type { LoginResponse } from "@/types/auth";
import { FormInput } from "@/components/FormInput";
import { LoadingSpinner } from "@/components/LoadingSpinner";

export default function LoginPage() {
  const router = useRouter();
  const [username, setUsername] = useState("admin");
  const [password, setPassword] = useState("Admin@12345");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleLogin = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");

    if (!username.trim() || !password.trim()) {
      setError("Username and password are required.");
      return;
    }

    setLoading(true);

    try {
      const response = await api.post<LoginResponse>("/auth/login", { username, password });
      setSession(normalizeAuthResponse(response.data));
      router.replace("/dashboard");
    } catch (apiError) {
      setDemoSession(username);
      setError(`${getApiErrorMessage(apiError)} Demo session started with fallback data.`);
      router.replace("/dashboard");
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="min-h-screen bg-slate-100 px-4 py-8">
      <div className="mx-auto grid min-h-[calc(100vh-4rem)] w-full max-w-6xl items-center gap-8 lg:grid-cols-[1fr_420px]">
        <section className="hidden lg:block">
          <div className="max-w-xl">
            <div className="mb-5 inline-flex rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white">RT IMS</div>
            <h1 className="text-5xl font-black leading-tight text-slate-950">RFID Yarn Store Management System</h1>
            <p className="mt-5 text-lg font-medium leading-8 text-slate-600">
              Warehouse receiving, stock confirmation, requisition picking, gate issue, returns, reports, and user access in one admin console.
            </p>
            <div className="mt-8 grid grid-cols-3 gap-3 text-center">
              {["Gate Scan", "MRR Flow", "Stock Trace"].map((item) => (
                <div key={item} className="rounded-lg border border-slate-200 bg-white p-4 text-sm font-bold text-slate-700 shadow-sm">
                  {item}
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm sm:p-8">
          <div className="mb-6">
            <h2 className="text-2xl font-black text-slate-950">Sign in</h2>
            <p className="mt-1 text-sm font-medium text-slate-500">RFID warehouse admin access</p>
          </div>

          <form onSubmit={handleLogin} className="space-y-4">
            <FormInput label="Username" value={username} onChange={(event) => setUsername(event.target.value)} autoComplete="username" />
            <FormInput
              label="Password"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              autoComplete="current-password"
            />

            {error ? (
              <div className="rounded-md border border-amber-200 bg-amber-50 px-3 py-2 text-sm font-semibold text-amber-800">{error}</div>
            ) : null}

            <button
              type="submit"
              disabled={loading}
              className="h-11 w-full rounded-md bg-blue-600 px-4 text-sm font-bold text-white shadow-sm hover:bg-blue-700 disabled:cursor-not-allowed disabled:bg-blue-300"
            >
              {loading ? "Signing in" : "Login"}
            </button>
          </form>

          {loading ? <LoadingSpinner label="Contacting API" /> : null}
        </section>
      </div>
    </main>
  );
}
