"use client";

import { useEffect, useSyncExternalStore, type ReactNode } from "react";
import { usePathname, useRouter } from "next/navigation";
import { getAuthSnapshot, getServerAuthSnapshot, subscribeAuth } from "@/lib/auth";
import { LoadingSpinner } from "./LoadingSpinner";
import { Navbar } from "./Navbar";
import { Sidebar } from "./Sidebar";

const publicRoutes = ["/login"];

export function ProtectedLayout({ children }: { children: ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const isPublic = publicRoutes.includes(pathname);
  const authenticated = useSyncExternalStore(subscribeAuth, getAuthSnapshot, getServerAuthSnapshot);

  useEffect(() => {
    if (isPublic) {
      if (authenticated) {
        router.replace("/dashboard");
      }
      return;
    }

    if (!authenticated) {
      router.replace("/login");
    }
  }, [authenticated, isPublic, pathname, router]);

  if (isPublic) {
    return <>{children}</>;
  }

  if (!authenticated) {
    return (
      <div className="min-h-screen bg-slate-100">
        <LoadingSpinner label="Checking session" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-100 text-slate-900">
      <Sidebar />
      <div className="md:pl-64">
        <Navbar />
        <main className="mx-auto w-full max-w-7xl px-4 py-6 md:px-6">{children}</main>
      </div>
    </div>
  );
}
