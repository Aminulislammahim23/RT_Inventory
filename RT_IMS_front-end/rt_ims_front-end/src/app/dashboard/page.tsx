"use client";

import { useEffect, useMemo, useState } from "react";
import api, { withFallback } from "@/lib/api";
import { normalizeStockItems, normalizeWarnings } from "@/lib/api-contract";
import { fallbackStocks, fallbackSummary, fallbackWarnings } from "@/lib/fallback-data";
import { formatDate } from "@/lib/format";
import type { GateWarning } from "@/types/rfid";
import type { StockItem, StockSummary } from "@/types/stock";
import { DashboardCard } from "@/components/DashboardCard";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

export default function DashboardPage() {
  const [stocks, setStocks] = useState<StockItem[]>(fallbackStocks);
  const [warnings, setWarnings] = useState<GateWarning[]>(fallbackWarnings);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const loadDashboard = async () => {
      setLoading(true);

      const [stockResult, warningResult] = await Promise.all([
        withFallback<StockItem[]>(() => api.get("/stocks/current"), fallbackStocks, normalizeStockItems),
        withFallback<GateWarning[]>(
          () => api.get("/reports/warnings"),
          fallbackWarnings,
          normalizeWarnings,
        ),
      ]);

      setStocks(stockResult.data);
      setWarnings(warningResult.data);
      setError(stockResult.error || warningResult.error);
      setLoading(false);
    };

    loadDashboard();
  }, []);

  const summary = useMemo<StockSummary>(() => {
    if (!stocks.length) {
      return fallbackSummary;
    }

    return stocks.reduce(
      (acc, item) => {
        const status = item.status.toLowerCase();
        if (status === "parking") acc.parkingStock += item.bagQty;
        if (status === "confirmed") acc.confirmedStock += item.bagQty;
        if (status === "issued") acc.issuedStock += item.bagQty;
        if (status === "return") acc.returnStock += item.bagQty;
        return acc;
      },
      { parkingStock: 0, confirmedStock: 0, issuedStock: 0, returnStock: 0, warningCount: warnings.length },
    );
  }, [stocks, warnings.length]);

  const warningColumns = useMemo<DataTableColumn<GateWarning>[]>(
    () => [
      { header: "Type", render: (row) => <StatusBadge status={row.type} /> },
      { header: "Message", accessor: "message", className: "min-w-[320px]" },
      { header: "Reference", accessor: "referenceNo" },
      { header: "Severity", render: (row) => <StatusBadge status={row.severity} /> },
      { header: "Date", render: (row) => formatDate(row.createdAt) },
    ],
    [],
  );

  return (
    <div>
      <PageHeader title="Dashboard" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-5">
        <DashboardCard title="Parking Stock" value={summary.parkingStock} tone="yellow" caption="Bags awaiting MRR" />
        <DashboardCard title="Confirmed Stock" value={summary.confirmedStock} tone="green" caption="Approved bags" />
        <DashboardCard title="Issued Stock" value={summary.issuedStock} tone="blue" caption="Outgoing bags" />
        <DashboardCard title="Return Stock" value={summary.returnStock} tone="slate" caption="Returned bags" />
        <DashboardCard title="Warning Count" value={summary.warningCount} tone="red" caption="Gate exceptions" />
      </div>

      <section className="mt-6">
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-lg font-black text-slate-950">Recent Warnings</h2>
        </div>
        {loading ? <LoadingSpinner /> : <DataTable columns={warningColumns} data={warnings} rowKey={(row) => row.id} />}
      </section>
    </div>
  );
}
