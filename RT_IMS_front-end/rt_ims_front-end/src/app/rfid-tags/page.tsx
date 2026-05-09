"use client";

import { useEffect, useMemo, useState } from "react";
import api, { getApiErrorMessage, withFallback } from "@/lib/api";
import { normalizeAsnGrns, normalizeRfidTag, normalizeRfidTagHistoryList, normalizeRfidTags } from "@/lib/api-contract";
import { fallbackAsnGrns, fallbackRfidTags, fallbackTagHistory } from "@/lib/fallback-data";
import { formatDate } from "@/lib/format";
import type { RfidTag, RfidTagHistory } from "@/types/rfid";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { FormInput } from "@/components/FormInput";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { Modal } from "@/components/Modal";
import { PageHeader } from "@/components/PageHeader";
import { SelectInput } from "@/components/SelectInput";
import { StatusBadge } from "@/components/StatusBadge";

export default function RfidTagsPage() {
  const [tags, setTags] = useState<RfidTag[]>(fallbackRfidTags);
  const [scanTagNo, setScanTagNo] = useState("");
  const [asnRecords, setAsnRecords] = useState(fallbackAsnGrns);
  const [assignForm, setAssignForm] = useState({ tagValue: "", asnGrnId: String(fallbackAsnGrns[0].id), bagNumber: "" });
  const [history, setHistory] = useState<RfidTagHistory[]>([]);
  const [historyTitle, setHistoryTitle] = useState("");
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      const [tagResult, asnResult] = await Promise.all([
        withFallback<RfidTag[]>(() => api.get("/rfid-tags"), fallbackRfidTags, normalizeRfidTags),
        withFallback(() => api.get("/asn-grns"), fallbackAsnGrns, normalizeAsnGrns),
      ]);
      setTags(tagResult.data);
      setAsnRecords(asnResult.data);
      setError(tagResult.error || asnResult.error);
      setLoading(false);
    };

    load();
  }, []);

  const handleScan = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!scanTagNo.trim()) return;
    setSubmitting(true);
    setError("");

    const fallbackTag: RfidTag = {
      id: `tag-${Date.now()}`,
      tagNo: scanTagNo.trim(),
      status: "Scanned",
      currentStatus: "Ready for assignment",
      lastSeenAt: new Date().toISOString(),
    };

    try {
      const response = await api.post("/rfid-tags/scan", { tagValue: scanTagNo.trim() });
      const scanned = normalizeRfidTag(response.data);
      setTags((current) => [scanned, ...current.filter((tag) => tag.tagNo !== scanned.tagNo)]);
      setAssignForm((current) => ({ ...current, tagValue: scanned.tagNo }));
      setScanTagNo("");
    } catch (apiError) {
      setTags((current) => [fallbackTag, ...current]);
      setAssignForm((current) => ({ ...current, tagValue: fallbackTag.tagNo }));
      setError(getApiErrorMessage(apiError));
    } finally {
      setSubmitting(false);
    }
  };

  const handleAssign = async (event: React.FormEvent) => {
    event.preventDefault();
    setSubmitting(true);
    setError("");

    const asn = asnRecords.find((item) => String(item.id) === assignForm.asnGrnId) ?? asnRecords[0];
    const fallbackAssigned: RfidTag = {
      id: `tag-${Date.now()}`,
      tagNo: assignForm.tagValue,
      asnGrnId: asn.id,
      asnGrnNo: asn.asnGrnNo,
      bagNo: assignForm.bagNumber,
      lotNo: asn.lotNo,
      itemYarnType: asn.itemYarnType,
      status: "Active",
      currentStatus: asn.status,
      assignedAt: new Date().toISOString(),
      lastSeenAt: new Date().toISOString(),
    };

    try {
      const response = await api.post("/rfid-tags/assign", {
        tagValue: assignForm.tagValue.trim(),
        asnGrnId: Number(assignForm.asnGrnId),
        bagNumber: Number(assignForm.bagNumber),
        isLooseBag: false,
      });
      const assigned = normalizeRfidTag(response.data);
      setTags((current) => [assigned, ...current.filter((tag) => tag.tagNo !== assigned.tagNo)]);
      setAssignForm((current) => ({ ...current, tagValue: "", bagNumber: "" }));
    } catch (apiError) {
      setTags((current) => [fallbackAssigned, ...current.filter((tag) => tag.tagNo !== fallbackAssigned.tagNo)]);
      setError(getApiErrorMessage(apiError));
    } finally {
      setSubmitting(false);
    }
  };

  const toggleStatus = async (tag: RfidTag) => {
    const nextStatus = tag.status === "Active" ? "Inactive" : "Active";
    setTags((current) => current.map((item) => (item.id === tag.id ? { ...item, status: nextStatus } : item)));

    try {
      const response = await api.patch(`/rfid-tags/${tag.id}/status`, { isActive: nextStatus === "Active" });
      setTags((current) => current.map((item) => (item.id === tag.id ? normalizeRfidTag(response.data) : item)));
    } catch (apiError) {
      setError(getApiErrorMessage(apiError));
    }
  };

  const openHistory = async (tag: RfidTag) => {
    setHistoryTitle(tag.tagNo);
    const result = await withFallback<RfidTagHistory[]>(
      () => api.get(`/rfid-tags/${tag.id}/history`),
      fallbackTagHistory.map((item) => ({ ...item, tagNo: tag.tagNo })),
      normalizeRfidTagHistoryList,
    );
    setHistory(result.data);
    setError(result.error);
  };

  const columns = useMemo<DataTableColumn<RfidTag>[]>(
    () => [
      { header: "RFID Tag", accessor: "tagNo" },
      { header: "ASN/GRN No", accessor: "asnGrnNo" },
      { header: "Bag No", accessor: "bagNo" },
      { header: "Lot No", accessor: "lotNo" },
      { header: "Item/Yarn Type", accessor: "itemYarnType" },
      { header: "Status", render: (row) => <StatusBadge status={row.status} /> },
      { header: "Current", render: (row) => <StatusBadge status={row.currentStatus ?? row.status} /> },
      { header: "Last Seen", render: (row) => formatDate(row.lastSeenAt) },
      {
        header: "Actions",
        render: (row) => (
          <div className="flex gap-2">
            <button type="button" onClick={() => toggleStatus(row)} className="font-bold text-blue-700 hover:text-blue-900">
              {row.status === "Active" ? "Deactivate" : "Activate"}
            </button>
            <button type="button" onClick={() => openHistory(row)} className="font-bold text-slate-700 hover:text-slate-950">
              History
            </button>
          </div>
        ),
      },
    ],
    [],
  );

  const historyColumns = useMemo<DataTableColumn<RfidTagHistory>[]>(
    () => [
      { header: "Action", accessor: "action" },
      { header: "Reference", accessor: "referenceNo" },
      { header: "Location", accessor: "location" },
      { header: "User", accessor: "user" },
      { header: "Time", render: (row) => formatDate(row.timestamp) },
    ],
    [],
  );

  return (
    <div>
      <PageHeader title="RFID Tag Management" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <div className="grid gap-5 lg:grid-cols-2">
        <form onSubmit={handleScan} className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
          <h2 className="mb-4 text-lg font-black text-slate-950">Scan RFID Tag</h2>
          <div className="flex flex-col gap-3 sm:flex-row">
            <div className="flex-1">
              <FormInput label="RFID Tag No" value={scanTagNo} onChange={(event) => setScanTagNo(event.target.value)} />
            </div>
            <button
              type="submit"
              disabled={submitting}
              className="self-end rounded-md bg-blue-600 px-4 py-2.5 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300"
            >
              Scan
            </button>
          </div>
        </form>

        <form onSubmit={handleAssign} className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
          <h2 className="mb-4 text-lg font-black text-slate-950">Assign Tag</h2>
          <div className="grid gap-3 sm:grid-cols-3">
            <FormInput label="RFID Tag No" value={assignForm.tagValue} onChange={(event) => setAssignForm({ ...assignForm, tagValue: event.target.value })} />
            <SelectInput
              label="ASN/GRN"
              value={assignForm.asnGrnId}
              onChange={(event) => setAssignForm({ ...assignForm, asnGrnId: event.target.value })}
              options={asnRecords.map((item) => ({ label: item.asnGrnNo, value: String(item.id) }))}
            />
            <FormInput label="Bag No" type="number" min="1" value={assignForm.bagNumber} onChange={(event) => setAssignForm({ ...assignForm, bagNumber: event.target.value })} />
          </div>
          <div className="mt-4 flex justify-end">
            <button type="submit" disabled={submitting} className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300">
              Assign
            </button>
          </div>
        </form>
      </div>

      <section className="mt-6">{loading ? <LoadingSpinner /> : <DataTable columns={columns} data={tags} rowKey={(row) => row.id} />}</section>

      <Modal isOpen={Boolean(historyTitle)} title={`RFID History: ${historyTitle}`} onClose={() => setHistoryTitle("")}>
        <DataTable columns={historyColumns} data={history} rowKey={(row) => row.id} />
      </Modal>
    </div>
  );
}
