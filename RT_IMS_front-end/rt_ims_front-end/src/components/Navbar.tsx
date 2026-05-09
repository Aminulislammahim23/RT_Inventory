"use client";

import { usePathname } from "next/navigation";
import { getStoredUser } from "@/lib/auth";

const titles: Record<string, string> = {
  "/dashboard": "Dashboard",
  "/asn-grn": "ASN/GRN Management",
  "/rfid-tags": "RFID Tag Management",
  "/receive": "RFID Gate Receive",
  "/stock": "Stock Management",
  "/mrr-approval": "MRR Approval",
  "/requisitions": "Requisition / Picking List",
  "/gate-issue": "RFID Gate Issue",
  "/returns": "Return Management",
  "/reports": "Reports",
  "/users": "User Management",
};

export function Navbar() {
  const pathname = usePathname();
  const user = getStoredUser();
  const matchingTitle = Object.entries(titles).find(([path]) => pathname === path || pathname.startsWith(`${path}/`));

  return (
    <header className="sticky top-0 z-30 border-b border-slate-200 bg-white/95 px-4 py-3 backdrop-blur md:px-6">
      <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <p className="text-xs font-bold uppercase tracking-wide text-slate-500">Warehouse Admin</p>
          <h2 className="text-lg font-black text-slate-950">{matchingTitle?.[1] ?? "RFID Yarn Store"}</h2>
        </div>
        <div className="flex items-center gap-3">
          <span className="rounded-full bg-emerald-50 px-3 py-1 text-xs font-bold text-emerald-700 ring-1 ring-emerald-200">
            API Ready
          </span>
          <div className="text-right">
            <div className="text-sm font-bold text-slate-900">{user?.name ?? "Demo Admin"}</div>
            <div className="text-xs font-semibold text-slate-500">{user?.role ?? "Admin"}</div>
          </div>
        </div>
      </div>
    </header>
  );
}
