"use client";

import { useEffect, useMemo, useState } from "react";
import api, { withFallback } from "@/lib/api";
import { normalizeStockItems, normalizeStockTransactions } from "@/lib/api-contract";
import { fallbackStockTransactions, fallbackStocks } from "@/lib/fallback-data";
import { formatDate, formatWeight } from "@/lib/format";
import type { StockItem, StockTransaction } from "@/types/stock";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { FormInput } from "@/components/FormInput";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { PageHeader } from "@/components/PageHeader";
import { SelectInput } from "@/components/SelectInput";
import { StatusBadge } from "@/components/StatusBadge";

const tabs = ["Parking", "Confirmed", "Issued", "Return"];

export default function StockPage() {
  const [stocks, setStocks] = useState<StockItem[]>(fallbackStocks);
  const [transactions, setTransactions] = useState<StockTransaction[]>(fallbackStockTransactions);
  const [activeTab, setActiveTab] = useState("Parking");
  const [statusFilter, setStatusFilter] = useState("");
  const [itemFilter, setItemFilter] = useState("");
  const [lotFilter, setLotFilter] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      const [stockResult, transactionResult] = await Promise.all([
        withFallback<StockItem[]>(() => api.get("/stocks/current"), fallbackStocks, normalizeStockItems),
        withFallback<StockTransaction[]>(
          () => api.get("/stocks/transactions"),
          fallbackStockTransactions,
          normalizeStockTransactions,
        ),
      ]);

      setStocks(stockResult.data);
      setTransactions(transactionResult.data);
      setError(stockResult.error || transactionResult.error);
      setLoading(false);
    };

    load();
  }, []);

  const filteredStocks = useMemo(() => {
    return stocks.filter((stock) => {
      const tabMatch = stock.status.toLowerCase() === activeTab.toLowerCase();
      const statusMatch = !statusFilter || stock.status === statusFilter;
      const itemMatch = !itemFilter || stock.itemYarnType.toLowerCase().includes(itemFilter.toLowerCase());
      const lotMatch = !lotFilter || stock.lotNo.toLowerCase().includes(lotFilter.toLowerCase());
      return tabMatch && statusMatch && itemMatch && lotMatch;
    });
  }, [activeTab, itemFilter, lotFilter, statusFilter, stocks]);

  const stockColumns = useMemo<DataTableColumn<StockItem>[]>(
    () => [
      { header: "ASN/GRN No", accessor: "asnGrnNo" },
      { header: "Supplier", accessor: "supplier" },
      { header: "Lot", accessor: "lotNo" },
      { header: "Item/Yarn Type", accessor: "itemYarnType" },
      { header: "Bag Qty", render: (row) => row.bagQty },
      { header: "Weight/Bag", render: (row) => formatWeight(row.weightPerBag) },
      { header: "Total Weight", render: (row) => formatWeight(row.totalWeight) },
      { header: "Status", render: (row) => <StatusBadge status={row.status} /> },
    ],
    [],
  );

  const transactionColumns = useMemo<DataTableColumn<StockTransaction>[]>(
    () => [
      { header: "Reference", accessor: "referenceNo" },
      { header: "RFID Tag", accessor: "tagNo" },
      { header: "Transaction", render: (row) => <StatusBadge status={row.transactionType} /> },
      { header: "Lot", accessor: "lotNo" },
      { header: "Qty", render: (row) => row.quantity },
      { header: "Weight", render: (row) => formatWeight(row.weight) },
      { header: "User", accessor: "createdBy" },
      { header: "Date", render: (row) => formatDate(row.createdAt) },
    ],
    [],
  );

  return (
    <div>
      <PageHeader title="Stock Management" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <section className="mb-5 rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
        <div className="grid gap-4 md:grid-cols-3">
          <SelectInput
            label="Status"
            value={statusFilter}
            onChange={(event) => setStatusFilter(event.target.value)}
            options={[{ label: "All", value: "" }, ...tabs.map((tab) => ({ label: tab, value: tab }))]}
          />
          <FormInput label="Item/Yarn Type" value={itemFilter} onChange={(event) => setItemFilter(event.target.value)} />
          <FormInput label="Lot" value={lotFilter} onChange={(event) => setLotFilter(event.target.value)} />
        </div>
      </section>

      <div className="mb-5 flex gap-2 overflow-x-auto">
        {tabs.map((tab) => (
          <button
            type="button"
            key={tab}
            onClick={() => setActiveTab(tab)}
            className={`whitespace-nowrap rounded-md px-4 py-2 text-sm font-bold ${
              activeTab === tab ? "bg-blue-600 text-white" : "bg-white text-slate-700 ring-1 ring-slate-200 hover:bg-slate-50"
            }`}
          >
            {tab} Stock
          </button>
        ))}
      </div>

      {loading ? <LoadingSpinner /> : <DataTable columns={stockColumns} data={filteredStocks} rowKey={(row) => row.id} />}

      <section className="mt-6">
        <h2 className="mb-3 text-lg font-black text-slate-950">Stock Transaction History</h2>
        <DataTable columns={transactionColumns} data={transactions} rowKey={(row) => row.id} />
      </section>
    </div>
  );
}
