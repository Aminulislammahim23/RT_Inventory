"use client";

const statusStyles: Record<string, string> = {
  active: "bg-emerald-50 text-emerald-700 ring-emerald-200",
  assigned: "bg-blue-50 text-blue-700 ring-blue-200",
  confirmed: "bg-emerald-50 text-emerald-700 ring-emerald-200",
  inactive: "bg-red-50 text-red-700 ring-red-200",
  issued: "bg-blue-50 text-blue-700 ring-blue-200",
  parking: "bg-amber-50 text-amber-700 ring-amber-200",
  pending: "bg-amber-50 text-amber-700 ring-amber-200",
  picked: "bg-blue-50 text-blue-700 ring-blue-200",
  picking: "bg-amber-50 text-amber-700 ring-amber-200",
  rejected: "bg-red-50 text-red-700 ring-red-200",
  return: "bg-teal-50 text-teal-700 ring-teal-200",
  receive: "bg-blue-50 text-blue-700 ring-blue-200",
  approval: "bg-emerald-50 text-emerald-700 ring-emerald-200",
  deactivated: "bg-slate-100 text-slate-700 ring-slate-200",
  expired: "bg-red-50 text-red-700 ring-red-200",
  warning: "bg-amber-50 text-amber-700 ring-amber-200",
  danger: "bg-red-50 text-red-700 ring-red-200",
};

export function StatusBadge({ status }: { status: string }) {
  const key = status.toLowerCase();
  const style = statusStyles[key] ?? "bg-slate-100 text-slate-700 ring-slate-200";

  return (
    <span className={`inline-flex whitespace-nowrap rounded-full px-2.5 py-1 text-xs font-semibold ring-1 ${style}`}>
      {status}
    </span>
  );
}
