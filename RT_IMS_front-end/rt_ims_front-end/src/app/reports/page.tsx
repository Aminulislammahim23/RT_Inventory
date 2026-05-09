"use client";

import { useMemo, useState } from "react";
import api, { extractArray, getApiErrorMessage } from "@/lib/api";
import {
  cleanParams,
  normalizeRfidTagHistoryList,
  normalizeStockItems,
  normalizeWarnings,
} from "@/lib/api-contract";
import {
  fallbackAsnGrns,
  fallbackStockTransactions,
  fallbackStocks,
  fallbackTagHistory,
  fallbackWarnings,
} from "@/lib/fallback-data";
import { downloadCsv, toCsv } from "@/lib/format";
import type { GateWarning, RfidTagHistory } from "@/types/rfid";
import type { StockItem, StockTransaction } from "@/types/stock";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { FormInput } from "@/components/FormInput";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

type ReportRow = Record<string, string | number>;
type ReportFilters = { date: string; supplier: string; lot: string; item: string };
type ReportOption = {
  label: string;
  endpoint: string;
  fallback: ReportRow[];
  toRows: (payload: unknown) => ReportRow[];
  params: (filters: ReportFilters) => Record<string, string>;
};

const stockToRow = (item: StockItem): ReportRow => ({
  "ASN/GRN No": item.asnGrnNo,
  "RFID Tag": item.tagValue ?? "",
  Supplier: item.supplier,
  Lot: item.lotNo,
  "Item/Yarn Type": item.itemYarnType,
  Bags: item.bagQty,
  Weight: item.totalWeight,
  Status: item.status,
  Date: item.lastMovementAt ?? "",
});

const transactionToRow = (item: StockTransaction): ReportRow => ({
  Reference: item.referenceNo,
  "RFID Tag": item.tagNo,
  Lot: item.lotNo,
  "Item/Yarn Type": item.itemYarnType,
  Quantity: item.quantity,
  Weight: item.weight,
  User: item.createdBy,
  Date: item.createdAt,
});

const tagHistoryToRow = (item: RfidTagHistory): ReportRow => ({
  "RFID Tag": item.tagNo,
  Action: item.action,
  Reference: item.referenceNo ?? "",
  Location: item.location ?? "",
  User: item.user ?? "",
  Date: item.timestamp,
});

const warningToRow = (item: GateWarning): ReportRow => ({
  Type: item.type,
  Message: item.message,
  Severity: item.severity,
  Reference: item.referenceNo ?? "",
  Date: item.createdAt,
});

const stockRows = fallbackStocks.map(stockToRow);

const baseStockParams = (filters: ReportFilters) => ({
  itemType: filters.item,
  lot: filters.lot,
});

const reportOptions: ReportOption[] = [
  { label: "Current Stock", endpoint: "/reports/current-stock", fallback: stockRows, toRows: (payload: unknown) => normalizeStockItems(payload).map(stockToRow), params: baseStockParams },
  {
    label: "Parking Stock",
    endpoint: "/reports/parking-stock",
    fallback: stockRows.filter((row) => row.Status === "Parking"),
    toRows: (payload: unknown) => normalizeStockItems(payload).map(stockToRow),
    params: baseStockParams,
  },
  {
    label: "Confirmed Stock",
    endpoint: "/reports/confirmed-stock",
    fallback: stockRows.filter((row) => row.Status === "Confirmed"),
    toRows: (payload: unknown) => normalizeStockItems(payload).map(stockToRow),
    params: baseStockParams,
  },
  {
    label: "Issued Stock",
    endpoint: "/reports/issued-stock",
    fallback: stockRows.filter((row) => row.Status === "Issued"),
    toRows: (payload: unknown) => normalizeStockItems(payload).map(stockToRow),
    params: baseStockParams,
  },
  {
    label: "ASN-wise Receive",
    endpoint: "/reports/asn-wise-receive",
    fallback: fallbackAsnGrns.map<ReportRow>((item) => ({
      "ASN/GRN No": item.asnGrnNo,
      Supplier: item.supplier,
      Lot: item.lotNo,
      "Item/Yarn Type": item.itemYarnType,
      Bags: item.totalBagQty,
      Status: item.status,
      Date: item.createdAt ?? "",
    })),
    toRows: (payload: unknown) =>
      extractArray<Record<string, unknown>>(payload, []).map((item) => ({
        "ASN/GRN No": String(item.asnGrnNo ?? ""),
        Supplier: String(item.supplier ?? ""),
        Lot: String(item.lotNo ?? ""),
        "Item/Yarn Type": String(item.itemYarnType ?? ""),
        Bags: Number(item.receivedBagQty ?? 0),
        Weight: Number(item.receivedWeight ?? 0),
      })),
    params: (filters: ReportFilters) => ({
      fromDate: filters.date,
      supplier: filters.supplier,
      lot: filters.lot,
      itemType: filters.item,
    }),
  },
  {
    label: "Requisition-wise Issue",
    endpoint: "/reports/requisition-wise-issue",
    fallback: fallbackStockTransactions.map(transactionToRow),
    toRows: (payload: unknown) =>
      extractArray<Record<string, unknown>>(payload, []).map((item) => ({
        Requisition: String(item.requisitionNo ?? ""),
        Bags: Number(item.issuedBagQty ?? 0),
        Weight: Number(item.issuedWeight ?? 0),
      })),
    params: () => ({}),
  },
  {
    label: "RFID Tag History",
    endpoint: "/reports/rfid-tag-history",
    fallback: fallbackTagHistory.map(tagHistoryToRow),
    toRows: (payload: unknown) => normalizeRfidTagHistoryList(payload).map(tagHistoryToRow),
    params: () => ({}),
  },
  {
    label: "Warning Report",
    endpoint: "/reports/warnings",
    fallback: fallbackWarnings.map(warningToRow),
    toRows: (payload: unknown) => normalizeWarnings(payload).map(warningToRow),
    params: (filters: ReportFilters) => ({
      fromDate: filters.date,
    }),
  },
];

export default function ReportsPage() {
  const [selected, setSelected] = useState(reportOptions[0]);
  const [rows, setRows] = useState<ReportRow[]>(reportOptions[0].fallback);
  const [filters, setFilters] = useState({ date: "", supplier: "", lot: "", item: "" });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const runReport = async (option: (typeof reportOptions)[number]) => {
    setSelected(option);
    setLoading(true);
    setError("");

    try {
      const response = await api.get(option.endpoint, { params: cleanParams(option.params(filters)) });
      setRows(option.toRows(response.data));
    } catch (apiError) {
      setRows(option.fallback);
      setError(getApiErrorMessage(apiError));
    } finally {
      setLoading(false);
    }
  };

  const filteredRows = useMemo(() => {
    return rows.filter((row) => {
      const supplierMatch = !filters.supplier || String(row.Supplier ?? "").toLowerCase().includes(filters.supplier.toLowerCase());
      const lotMatch = !filters.lot || String(row.Lot ?? "").toLowerCase().includes(filters.lot.toLowerCase());
      const itemMatch = !filters.item || String(row["Item/Yarn Type"] ?? "").toLowerCase().includes(filters.item.toLowerCase());
      const dateMatch = !filters.date || String(row.Date ?? "").startsWith(filters.date);
      return supplierMatch && lotMatch && itemMatch && dateMatch;
    });
  }, [filters, rows]);

  const columns = useMemo<DataTableColumn<ReportRow>[]>(() => {
    const keys = Object.keys(filteredRows[0] ?? rows[0] ?? {});
    return keys.map((key) => ({
      header: key,
      render: (row) => {
        const value = row[key];
        if (key.toLowerCase().includes("status") || key.toLowerCase().includes("severity")) {
          return <StatusBadge status={String(value)} />;
        }
        return String(value ?? "");
      },
      className: key === "Message" ? "min-w-[360px]" : "",
    }));
  }, [filteredRows, rows]);

  const exportCsv = () => {
    downloadCsv(`${selected.label.toLowerCase().replaceAll(" ", "-")}.csv`, toCsv(filteredRows));
  };

  return (
    <div>
      <PageHeader
        title="Reports"
        actions={
          <button type="button" onClick={exportCsv} className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700">
            Export CSV
          </button>
        }
      />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <section className="mb-5 grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
        {reportOptions.map((option) => (
          <button
            key={option.endpoint}
            type="button"
            onClick={() => runReport(option)}
            className={`rounded-lg border p-4 text-left text-sm font-black shadow-sm transition ${
              selected.endpoint === option.endpoint ? "border-blue-600 bg-blue-50 text-blue-800" : "border-slate-200 bg-white text-slate-800 hover:border-blue-200"
            }`}
          >
            {option.label}
          </button>
        ))}
      </section>

      <section className="mb-5 rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
        <div className="grid gap-4 md:grid-cols-4">
          <FormInput label="Date" type="date" value={filters.date} onChange={(event) => setFilters({ ...filters, date: event.target.value })} />
          <FormInput label="Supplier" value={filters.supplier} onChange={(event) => setFilters({ ...filters, supplier: event.target.value })} />
          <FormInput label="Lot" value={filters.lot} onChange={(event) => setFilters({ ...filters, lot: event.target.value })} />
          <FormInput label="Item" value={filters.item} onChange={(event) => setFilters({ ...filters, item: event.target.value })} />
        </div>
      </section>

      {loading ? (
        <div className="rounded-lg border border-slate-200 bg-white p-8 text-center text-sm font-bold text-slate-500">Loading report</div>
      ) : (
        <DataTable columns={columns} data={filteredRows} rowKey={(_, index) => index} />
      )}
    </div>
  );
}
