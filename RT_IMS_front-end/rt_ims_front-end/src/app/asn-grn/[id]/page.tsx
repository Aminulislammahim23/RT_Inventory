"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import api, { withFallback } from "@/lib/api";
import { normalizeAsnGrn, normalizeRfidTags } from "@/lib/api-contract";
import { fallbackAsnGrns, fallbackRfidTags } from "@/lib/fallback-data";
import { formatDate } from "@/lib/format";
import type { AsnGrn } from "@/types/asn";
import type { RfidTag } from "@/types/rfid";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

export default function AsnGrnDetailPage() {
  const router = useRouter();
  const params = useParams<{ id: string }>();
  const [record, setRecord] = useState<AsnGrn | null>(null);
  const [allTags, setAllTags] = useState<RfidTag[]>(fallbackRfidTags);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const id = params.id;
    const fallback = fallbackAsnGrns.find((item) => item.id === id) ?? fallbackAsnGrns[0];

    const load = async () => {
      const [recordResult, tagResult] = await Promise.all([
        withFallback<AsnGrn>(() => api.get(`/asn-grns/${id}`), fallback, normalizeAsnGrn),
        withFallback<RfidTag[]>(() => api.get("/rfid-tags"), fallbackRfidTags, normalizeRfidTags),
      ]);
      setRecord(recordResult.data);
      setAllTags(tagResult.data);
      setError(recordResult.error || tagResult.error);
      setLoading(false);
    };

    load();
  }, [params.id]);

  const tags = useMemo(
    () => record?.assignedTags ?? allTags.filter((tag) => String(tag.asnGrnId) === String(record?.id) || tag.asnGrnNo === record?.asnGrnNo),
    [allTags, record],
  );

  const tagColumns = useMemo<DataTableColumn<RfidTag>[]>(
    () => [
      { header: "RFID Tag", accessor: "tagNo" },
      { header: "Bag No", accessor: "bagNo" },
      { header: "Lot No", accessor: "lotNo" },
      { header: "Current Status", render: (row) => <StatusBadge status={row.currentStatus ?? row.status} /> },
      { header: "Last Seen", render: (row) => formatDate(row.lastSeenAt) },
    ],
    [],
  );

  if (loading) {
    return <LoadingSpinner />;
  }

  if (!record) {
    return (
      <div>
        <PageHeader title="ASN/GRN Detail" />
        <div className="rounded-lg border border-slate-200 bg-white p-6 text-sm font-semibold text-slate-600">Record not found.</div>
      </div>
    );
  }

  return (
    <div>
      <PageHeader
        title={record.asnGrnNo}
        actions={
          <>
            <button
              type="button"
              onClick={() => router.back()}
              className="rounded-md border border-slate-300 px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-50"
            >
              Back
            </button>
            <Link href={`/asn-grn/create?edit=${record.id}`} className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700">
              Edit
            </Link>
          </>
        }
      />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <section className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          {[
            ["Supplier", record.supplier],
            ["PO No", record.poNo],
            ["Challan No", record.challanNo],
            ["Lot No", record.lotNo],
            ["Item/Yarn Type", record.itemYarnType],
            ["Total Bag Qty", record.totalBagQty],
            ["Weight Per Bag", `${record.weightPerBag} kg`],
            ["Created", formatDate(record.createdAt)],
          ].map(([label, value]) => (
            <div key={String(label)} className="rounded-md border border-slate-200 bg-slate-50 p-3">
              <div className="text-xs font-bold uppercase tracking-wide text-slate-500">{label}</div>
              <div className="mt-1 text-sm font-bold text-slate-900">{value}</div>
            </div>
          ))}
        </div>
        <div className="mt-4">
          <StatusBadge status={record.status} />
        </div>
      </section>

      <section className="mt-6">
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-lg font-black text-slate-950">Assigned RFID Tags</h2>
        </div>
        <DataTable columns={tagColumns} data={tags} rowKey={(row) => row.id} />
      </section>
    </div>
  );
}
