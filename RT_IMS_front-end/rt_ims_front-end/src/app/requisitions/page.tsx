"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import api, { getApiErrorMessage, withFallback } from "@/lib/api";
import { normalizePickingList, normalizeRequisitions } from "@/lib/api-contract";
import { fallbackRequisitions } from "@/lib/fallback-data";
import { formatDate } from "@/lib/format";
import type { Requisition } from "@/types/requisition";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

export default function RequisitionsPage() {
  const [records, setRecords] = useState<Requisition[]>(fallbackRequisitions);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      const result = await withFallback<Requisition[]>(
        () => api.get("/requisitions"),
        fallbackRequisitions,
        normalizeRequisitions,
      );
      setRecords(result.data);
      setError(result.error);
      setLoading(false);
    };

    load();
  }, []);

  const createPickingList = async (record: Requisition) => {
    try {
      const response = await api.post(`/requisitions/${record.id}/picking-list`);
      const pickingList = normalizePickingList(response.data);
      setRecords((current) =>
        current.map((item) =>
          item.id === record.id ? { ...item, status: "Picked", pickingLists: [pickingList, ...(item.pickingLists ?? [])] } : item,
        ),
      );
    } catch (apiError) {
      setError(getApiErrorMessage(apiError));
      setRecords((current) => current.map((item) => (item.id === record.id ? { ...item, status: "Picked" } : item)));
    }
  };

  const columns = useMemo<DataTableColumn<Requisition>[]>(
    () => [
      { header: "Requisition No", accessor: "requisitionNo" },
      { header: "Purpose", accessor: "purpose" },
      { header: "Status", render: (row) => <StatusBadge status={row.status} /> },
      { header: "Created By", accessor: "createdBy" },
      { header: "Date", render: (row) => formatDate(row.date) },
      { header: "Picking List", render: (row) => row.pickingLists?.[0]?.pickingListNo ?? "-" },
      {
        header: "Actions",
        render: (row) => (
          <div className="flex gap-2">
            <button type="button" onClick={() => createPickingList(row)} className="font-bold text-blue-700 hover:text-blue-900">
              Picking List
            </button>
          </div>
        ),
      },
    ],
    [],
  );

  return (
    <div>
      <PageHeader
        title="Requisition / Picking List"
        actions={
          <Link href="/requisitions/create" className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700">
            Create Requisition
          </Link>
        }
      />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}
      {loading ? <LoadingSpinner /> : <DataTable columns={columns} data={records} rowKey={(row) => row.id} />}
    </div>
  );
}
