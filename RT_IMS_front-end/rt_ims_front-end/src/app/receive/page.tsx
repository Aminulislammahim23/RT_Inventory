"use client";

import { useMemo, useState } from "react";
import api, { getApiErrorMessage } from "@/lib/api";
import { normalizeReceiveScan, normalizeReceiveSession, receiveScanToTag, scanToWarning } from "@/lib/api-contract";
import { fallbackAsnGrns, fallbackRfidTags } from "@/lib/fallback-data";
import { formatDate } from "@/lib/format";
import type { GateWarning, ReceiveSession, RfidTag } from "@/types/rfid";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { FormInput } from "@/components/FormInput";
import { PageHeader } from "@/components/PageHeader";
import { StatusBadge } from "@/components/StatusBadge";

export default function ReceivePage() {
  const [asnGrnNo, setAsnGrnNo] = useState(fallbackAsnGrns[0].asnGrnNo);
  const [tagNo, setTagNo] = useState("");
  const [session, setSession] = useState<ReceiveSession | null>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const startSession = async (event: React.FormEvent) => {
    event.preventDefault();
    setLoading(true);
    setError("");

    const fallbackSession: ReceiveSession = {
      id: `recv-${Date.now()}`,
      asnGrnNo,
      status: "Active",
      startedAt: new Date().toISOString(),
      receivedTags: [],
      warnings: [],
    };

    try {
      const response = await api.post<ReceiveSession>("/receive-sessions", { asnGrnNo });
      setSession(response.data?.id ? normalizeReceiveSession(response.data) : fallbackSession);
    } catch (apiError) {
      const asnExists = fallbackAsnGrns.some((item) => item.asnGrnNo === asnGrnNo);
      fallbackSession.warnings = asnExists
        ? []
        : [
            {
              id: `warn-${Date.now()}`,
              type: "Missing ASN",
              message: `${asnGrnNo} was not found in ASN/GRN records.`,
              severity: "warning",
              referenceNo: asnGrnNo,
              createdAt: new Date().toISOString(),
            },
          ];
      setSession(fallbackSession);
      setError(getApiErrorMessage(apiError));
    } finally {
      setLoading(false);
    }
  };

  const scanTag = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!session || !tagNo.trim()) return;
    setLoading(true);
    setError("");

    try {
      const response = await api.post(`/receive-sessions/${session.id}/scan`, { tagValue: tagNo.trim() });
      const scan = normalizeReceiveScan(response.data);
      setSession((current) =>
        current
          ? {
              ...current,
              receivedTags:
                scan.isValid && !current.receivedTags.some((tag) => tag.tagNo === scan.tagValue)
                  ? [receiveScanToTag(scan), ...current.receivedTags]
                  : current.receivedTags,
              warnings: scan.isValid ? current.warnings : [scanToWarning(scan), ...current.warnings],
            }
          : current,
      );
    } catch (apiError) {
      const scannedTag = fallbackRfidTags.find((tag) => tag.tagNo === tagNo.trim());
      const duplicate = session.receivedTags.some((tag) => tag.tagNo === tagNo.trim());
      const warnings: GateWarning[] = [];

      if (!scannedTag) {
        warnings.push({
          id: `warn-${Date.now()}-missing`,
          type: "Missing ASN",
          message: `${tagNo.trim()} is not assigned to any ASN/GRN.`,
          severity: "warning",
          referenceNo: tagNo.trim(),
          createdAt: new Date().toISOString(),
        });
      } else if (scannedTag.asnGrnNo !== session.asnGrnNo) {
        warnings.push({
          id: `warn-${Date.now()}-mismatch`,
          type: "Mismatch Tag",
          message: `${tagNo.trim()} belongs to ${scannedTag.asnGrnNo}.`,
          severity: "danger",
          referenceNo: session.asnGrnNo,
          createdAt: new Date().toISOString(),
        });
      } else if (duplicate) {
        warnings.push({
          id: `warn-${Date.now()}-duplicate`,
          type: "Duplicate Scan",
          message: `${tagNo.trim()} has already been received in this session.`,
          severity: "warning",
          referenceNo: session.asnGrnNo,
          createdAt: new Date().toISOString(),
        });
      }

      setSession((current) =>
        current
          ? {
              ...current,
              receivedTags: scannedTag && !duplicate ? [{ ...scannedTag, lastSeenAt: new Date().toISOString() }, ...current.receivedTags] : current.receivedTags,
              warnings: [...warnings, ...current.warnings],
            }
          : current,
      );
      setError(getApiErrorMessage(apiError));
    } finally {
      setTagNo("");
      setLoading(false);
    }
  };

  const deactivate = async () => {
    if (!session) return;
    setLoading(true);

    try {
      const response = await api.post(`/receive-sessions/${session.id}/deactivate`);
      setSession(normalizeReceiveSession(response.data));
    } catch (apiError) {
      setError(getApiErrorMessage(apiError));
      setSession((current) => (current ? { ...current, status: "Inactive" } : current));
    } finally {
      setLoading(false);
    }
  };

  const tagColumns = useMemo<DataTableColumn<RfidTag>[]>(
    () => [
      { header: "RFID Tag", accessor: "tagNo" },
      { header: "Bag No", accessor: "bagNo" },
      { header: "Lot No", accessor: "lotNo" },
      { header: "Item/Yarn Type", accessor: "itemYarnType" },
      { header: "Received At", render: (row) => formatDate(row.lastSeenAt) },
    ],
    [],
  );

  const warningColumns = useMemo<DataTableColumn<GateWarning>[]>(
    () => [
      { header: "Type", render: (row) => <StatusBadge status={row.type} /> },
      { header: "Message", accessor: "message", className: "min-w-[320px]" },
      { header: "Severity", render: (row) => <StatusBadge status={row.severity} /> },
      { header: "Time", render: (row) => formatDate(row.createdAt) },
    ],
    [],
  );

  return (
    <div>
      <PageHeader title="RFID Gate Receive" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <div className="grid gap-5 lg:grid-cols-[380px_1fr]">
        <section className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
          <form onSubmit={startSession} className="space-y-4">
            <FormInput label="ASN/GRN Scan" value={asnGrnNo} onChange={(event) => setAsnGrnNo(event.target.value)} />
            <button type="submit" disabled={loading} className="w-full rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300">
              Start Receive Session
            </button>
          </form>

          {session ? (
            <div className="mt-5 space-y-3 rounded-md border border-slate-200 bg-slate-50 p-4">
              <div className="flex items-center justify-between">
                <span className="text-sm font-bold text-slate-700">{session.asnGrnNo}</span>
                <StatusBadge status={session.status} />
              </div>
              <div className="text-xs font-semibold text-slate-500">{formatDate(session.startedAt)}</div>
              <button type="button" onClick={deactivate} className="w-full rounded-md bg-red-600 px-4 py-2 text-sm font-bold text-white hover:bg-red-700">
                Deactivate Session
              </button>
            </div>
          ) : null}
        </section>

        <section className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
          <form onSubmit={scanTag} className="flex flex-col gap-3 sm:flex-row">
            <div className="flex-1">
              <FormInput label="RFID Tag Scan" value={tagNo} onChange={(event) => setTagNo(event.target.value)} disabled={!session || session.status !== "Active"} />
            </div>
            <button
              type="submit"
              disabled={!session || session.status !== "Active" || loading}
              className="self-end rounded-md bg-blue-600 px-4 py-2.5 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300"
            >
              Receive
            </button>
          </form>
        </section>
      </div>

      <section className="mt-6">
        <h2 className="mb-3 text-lg font-black text-slate-950">Received Bags</h2>
        <DataTable columns={tagColumns} data={session?.receivedTags ?? []} rowKey={(row) => row.id} />
      </section>

      <section className="mt-6">
        <h2 className="mb-3 text-lg font-black text-slate-950">Warnings</h2>
        <DataTable columns={warningColumns} data={session?.warnings ?? []} rowKey={(row) => row.id} />
      </section>
    </div>
  );
}
