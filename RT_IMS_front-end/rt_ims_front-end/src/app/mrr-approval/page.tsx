"use client";

import { useEffect, useMemo, useState } from "react";
import api, { getApiErrorMessage, withFallback } from "@/lib/api";
import { normalizeAsnGrns } from "@/lib/api-contract";
import { fallbackAsnGrns } from "@/lib/fallback-data";
import type { AsnGrn } from "@/types/asn";
import { ConfirmDialog } from "@/components/ConfirmDialog";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

export default function MrrApprovalPage() {
  const [records, setRecords] = useState<AsnGrn[]>(fallbackAsnGrns.filter((item) => item.status === "Parking"));
  const [selected, setSelected] = useState<AsnGrn | null>(null);
  const [loading, setLoading] = useState(true);
  const [approving, setApproving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      const result = await withFallback<AsnGrn[]>(
        () => api.get("/asn-grns"),
        fallbackAsnGrns,
        normalizeAsnGrns,
      );
      setRecords(result.data.filter((item) => item.status.toLowerCase() === "parking"));
      setError(result.error);
      setLoading(false);
    };

    load();
  }, []);

  const approve = async () => {
    if (!selected) return;
    setApproving(true);

    try {
      await api.post(`/mrr-approvals/${selected.id}/approve`);
    } catch (apiError) {
      setError(getApiErrorMessage(apiError));
    } finally {
      setRecords((current) => current.filter((item) => item.id !== selected.id));
      setSelected(null);
      setApproving(false);
    }
  };

  const columns = useMemo<DataTableColumn<AsnGrn>[]>(
    () => [
      { header: "ASN/GRN No", accessor: "asnGrnNo" },
      { header: "Supplier", accessor: "supplier" },
      { header: "PO No", accessor: "poNo" },
      { header: "Lot No", accessor: "lotNo" },
      { header: "Item/Yarn Type", accessor: "itemYarnType" },
      { header: "Bag Qty", render: (row) => row.totalBagQty },
      { header: "Status", render: (row) => <StatusBadge status={row.status} /> },
      {
        header: "Actions",
        render: (row) => (
          <button type="button" onClick={() => setSelected(row)} className="rounded-md bg-blue-600 px-3 py-1.5 text-xs font-bold text-white hover:bg-blue-700">
            Approve
          </button>
        ),
      },
    ],
    [],
  );

  return (
    <div>
      <PageHeader title="MRR Approval" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}
      {loading ? <LoadingSpinner /> : <DataTable columns={columns} data={records} rowKey={(row) => row.id} emptyMessage="No parking ASN/GRN records pending approval" />}

      <ConfirmDialog
        isOpen={Boolean(selected)}
        title="Approve MRR"
        message={`Approve ${selected?.asnGrnNo ?? "selected ASN/GRN"} and move bags to confirmed stock?`}
        confirmLabel="Approve"
        loading={approving}
        onCancel={() => setSelected(null)}
        onConfirm={approve}
      />
    </div>
  );
}
