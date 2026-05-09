"use client";

type DashboardCardProps = {
  title: string;
  value: string | number;
  tone?: "blue" | "green" | "yellow" | "red" | "slate";
  caption?: string;
};

const toneStyles = {
  blue: "border-blue-100 bg-blue-50 text-blue-700",
  green: "border-emerald-100 bg-emerald-50 text-emerald-700",
  yellow: "border-amber-100 bg-amber-50 text-amber-700",
  red: "border-red-100 bg-red-50 text-red-700",
  slate: "border-slate-100 bg-slate-50 text-slate-700",
};

export function DashboardCard({ title, value, tone = "blue", caption }: DashboardCardProps) {
  return (
    <section className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
      <div className={`mb-4 inline-flex rounded-md border px-3 py-1 text-xs font-bold ${toneStyles[tone]}`}>{title}</div>
      <div className="text-3xl font-black text-slate-950">{value}</div>
      {caption ? <p className="mt-2 text-sm font-medium text-slate-500">{caption}</p> : null}
    </section>
  );
}
