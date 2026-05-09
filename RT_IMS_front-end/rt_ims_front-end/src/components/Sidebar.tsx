"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { clearSession } from "@/lib/auth";

const menu = [
  { label: "Dashboard", href: "/dashboard" },
  { label: "ASN/GRN", href: "/asn-grn" },
  { label: "RFID Tags", href: "/rfid-tags" },
  { label: "Gate Receive", href: "/receive" },
  { label: "Stock", href: "/stock" },
  { label: "MRR Approval", href: "/mrr-approval" },
  { label: "Requisition", href: "/requisitions" },
  { label: "Gate Issue", href: "/gate-issue" },
  { label: "Return", href: "/returns" },
  { label: "Reports", href: "/reports" },
  { label: "Users", href: "/users" },
];

export function Sidebar() {
  const pathname = usePathname();
  const router = useRouter();

  const logout = () => {
    clearSession();
    router.replace("/login");
  };

  return (
    <aside className="sticky top-0 z-40 w-full bg-slate-950 text-white md:fixed md:bottom-0 md:left-0 md:h-screen md:w-64">
      <div className="flex h-full flex-col">
        <div className="border-b border-white/10 px-5 py-4">
          <div className="text-lg font-black tracking-wide">RT IMS</div>
          <div className="mt-1 text-xs font-semibold text-blue-200">RFID Yarn Store</div>
        </div>
        <nav className="flex gap-1 overflow-x-auto px-3 py-3 md:flex-1 md:flex-col md:overflow-visible">
          {menu.map((item) => {
            const active = pathname === item.href || pathname.startsWith(`${item.href}/`);

            return (
              <Link
                key={item.href}
                href={item.href}
                className={`whitespace-nowrap rounded-md px-3 py-2 text-sm font-semibold transition md:px-4 ${
                  active ? "bg-blue-600 text-white" : "text-slate-200 hover:bg-white/10 hover:text-white"
                }`}
              >
                {item.label}
              </Link>
            );
          })}
        </nav>
        <div className="border-t border-white/10 p-3">
          <button
            type="button"
            onClick={logout}
            className="w-full rounded-md px-3 py-2 text-left text-sm font-semibold text-red-100 hover:bg-red-500/20 hover:text-white"
          >
            Logout
          </button>
        </div>
      </div>
    </aside>
  );
}
