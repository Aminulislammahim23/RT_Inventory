"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import api, { withFallback } from "@/lib/api";
import { normalizeAsnGrns } from "@/lib/api-contract";
import { fallbackAsnGrns } from "@/lib/fallback-data";
import type { AsnGrn } from "@/types/asn";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

export default function AsnGrnPage() {
  const [records, setRecords] = useState<AsnGrn[]>(fallbackAsnGrns);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      const result = await withFallback<AsnGrn[]>(
        () => api.get("/asn-grns"),
        fallbackAsnGrns,
        normalizeAsnGrns,
      );

      setRecords(result.data);
      setError(result.error);
      setLoading(false);
    };

    load();
  }, []);

  const columns = useMemo<DataTableColumn<AsnGrn>[]>(
    () => [
      { header: "ASN/GRN No", accessor: "asnGrnNo" },
      { header: "Supplier", accessor: "supplier" },
      { header: "PO No", accessor: "poNo" },
      { header: "Challan No", accessor: "challanNo" },
      { header: "Lot No", accessor: "lotNo" },
      { header: "Item/Yarn Type", accessor: "itemYarnType" },
      { header: "Total Bag Qty", render: (row) => row.totalBagQty },
      { header: "Weight Per Bag", render: (row) => `${row.weightPerBag} kg` },
      { header: "Status", render: (row) => <StatusBadge status={row.status} /> },
      {
        header: "Actions",
        render: (row) => (
          <Link href={`/asn-grn/${row.id}`} className="font-bold text-blue-700 hover:text-blue-900">
            View
          </Link>
        ),
      },
    ],
    [],
  );

  return (
    <div>
      <PageHeader
        title="ASN/GRN Management"
        actions={
          <Link href="/asn-grn/create" className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700">
            Create
          </Link>
        }
      />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}
      {loading ? <LoadingSpinner /> : <DataTable columns={columns} data={records} rowKey={(row) => row.id} />}
    </div>
  );
}
