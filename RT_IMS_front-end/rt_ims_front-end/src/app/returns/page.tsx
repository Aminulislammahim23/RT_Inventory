"use client";

import { useEffect, useMemo, useState } from "react";
import api, { getApiErrorMessage, withFallback } from "@/lib/api";
import { normalizeReturn, normalizeReturns } from "@/lib/api-contract";
import { fallbackReturns } from "@/lib/fallback-data";
import { formatDate } from "@/lib/format";
import type { ReturnRecord } from "@/types/returns";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { FormInput } from "@/components/FormInput";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

export default function ReturnsPage() {
  const [records, setRecords] = useState<ReturnRecord[]>(fallbackReturns);
  const [form, setForm] = useState({ sourceRequisitionId: "", lotNo: "", itemYarnType: "", bagQty: "1", weightPerBag: "0" });
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      const result = await withFallback<ReturnRecord[]>(() => api.get("/returns"), fallbackReturns, normalizeReturns);
      setRecords(result.data);
      setError(result.error);
    };

    load();
  }, []);

  const createReturn = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError("");

    const fallbackRecord: ReturnRecord = {
      id: `ret-${Date.now()}`,
      returnNo: `RET-2026-${String(records.length + 10).padStart(4, "0")}`,
      source: form.sourceRequisitionId ? `Requisition #${form.sourceRequisitionId}` : "Manual Return",
      sourceRequisitionId: form.sourceRequisitionId ? Number(form.sourceRequisitionId) : null,
      asnGrnNo: "",
      lotNo: form.lotNo,
      itemYarnType: form.itemYarnType,
      bagQty: Number(form.bagQty),
      weightPerBag: Number(form.weightPerBag),
      status: "Pending ASN",
      date: new Date().toISOString(),
    };

    try {
      const response = await api.post("/returns", {
        sourceRequisitionId: form.sourceRequisitionId ? Number(form.sourceRequisitionId) : null,
      });
      setRecords((current) => [
        {
          ...normalizeReturn(response.data),
          lotNo: form.lotNo,
          itemYarnType: form.itemYarnType,
          bagQty: Number(form.bagQty),
          weightPerBag: Number(form.weightPerBag),
        },
        ...current,
      ]);
      setForm({ sourceRequisitionId: "", lotNo: "", itemYarnType: "", bagQty: "1", weightPerBag: "0" });
    } catch (apiError) {
      setRecords((current) => [fallbackRecord, ...current]);
      setError(getApiErrorMessage(apiError));
    } finally {
      setSaving(false);
    }
  };

  const generateAsn = async (record: ReturnRecord) => {
    const asnGrnNo = record.asnGrnNo || `RTN-2026-${String(Date.now()).slice(-4)}`;

    try {
      const response = await api.post(`/returns/${record.id}/asn-grn`, {
        lotNo: record.lotNo,
        itemYarnType: record.itemYarnType,
        totalBagQty: record.bagQty,
        weightPerBag: record.weightPerBag ?? 0,
      });
      const updated = normalizeReturn(response.data);
      setRecords((current) =>
        current.map((item) =>
          item.id === record.id
            ? {
                ...updated,
                lotNo: record.lotNo,
                itemYarnType: record.itemYarnType,
                bagQty: record.bagQty,
                weightPerBag: record.weightPerBag,
                asnGrnNo: updated.asnGrnNo || asnGrnNo,
              }
            : item,
        ),
      );
    } catch (apiError) {
      setError(getApiErrorMessage(apiError));
      setRecords((current) => current.map((item) => (item.id === record.id ? { ...item, asnGrnNo, status: "Return ASN Created" } : item)));
    }
  };

  const columns = useMemo<DataTableColumn<ReturnRecord>[]>(
    () => [
      { header: "Return No", accessor: "returnNo" },
      { header: "Source", accessor: "source" },
      { header: "ASN/GRN", render: (row) => row.asnGrnNo || "-" },
      { header: "Lot", accessor: "lotNo" },
      { header: "Item/Yarn Type", accessor: "itemYarnType" },
      { header: "Bag Qty", render: (row) => row.bagQty },
      { header: "Status", render: (row) => <StatusBadge status={row.status} /> },
      { header: "Date", render: (row) => formatDate(row.date) },
      {
        header: "Actions",
        render: (row) => (
          <button
            type="button"
            onClick={() => generateAsn(row)}
            disabled={Boolean(row.asnGrnNo) || !row.lotNo || !row.itemYarnType}
            className="font-bold text-blue-700 hover:text-blue-900 disabled:text-slate-400"
          >
            Generate ASN/GRN
          </button>
        ),
      },
    ],
    [],
  );

  return (
    <div>
      <PageHeader title="Return Management" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <form onSubmit={createReturn} className="mb-6 rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
        <div className="grid gap-4 md:grid-cols-[1fr_1fr_1fr_110px_140px_120px]">
          <FormInput
            label="Source Req. ID"
            type="number"
            min="1"
            value={form.sourceRequisitionId}
            onChange={(event) => setForm({ ...form, sourceRequisitionId: event.target.value })}
          />
          <FormInput label="Lot" value={form.lotNo} onChange={(event) => setForm({ ...form, lotNo: event.target.value })} required />
          <FormInput label="Item/Yarn Type" value={form.itemYarnType} onChange={(event) => setForm({ ...form, itemYarnType: event.target.value })} required />
          <FormInput label="Bag Qty" type="number" min="1" value={form.bagQty} onChange={(event) => setForm({ ...form, bagQty: event.target.value })} required />
          <FormInput
            label="Weight/Bag"
            type="number"
            min="0"
            step="0.01"
            value={form.weightPerBag}
            onChange={(event) => setForm({ ...form, weightPerBag: event.target.value })}
            required
          />
          <button type="submit" disabled={saving} className="self-end rounded-md bg-blue-600 px-4 py-2.5 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300">
            Create
          </button>
        </div>
      </form>

      <DataTable columns={columns} data={records} rowKey={(row) => row.id} />
    </div>
  );
}
