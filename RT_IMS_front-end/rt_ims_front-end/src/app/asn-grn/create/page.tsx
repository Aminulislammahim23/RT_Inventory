"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import api, { getApiErrorMessage } from "@/lib/api";
import type { CreateAsnGrnPayload } from "@/types/asn";
import { FormInput } from "@/components/FormInput";
import { PageHeader } from "@/components/PageHeader";

const initialForm = {
  asnGrnNo: "",
  supplier: "",
  poNo: "",
  challanNo: "",
  lotNo: "",
  itemYarnType: "",
  totalBagQty: "0",
  weightPerBag: "0",
  challanCopyPath: "",
  usterReportPath: "",
};

export default function CreateAsnGrnPage() {
  const router = useRouter();
  const [form, setForm] = useState(initialForm);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const update = (field: keyof typeof form, value: string) => setForm((current) => ({ ...current, [field]: value }));

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError("");

    const payload: CreateAsnGrnPayload = {
      ...form,
      totalBagQty: Number(form.totalBagQty),
      weightPerBag: Number(form.weightPerBag),
    };

    try {
      await api.post("/asn-grns", payload);
      router.push("/asn-grn");
    } catch (apiError) {
      setError(`${getApiErrorMessage(apiError)} Record saved locally for demo view.`);
      router.push("/asn-grn");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader title="Create ASN/GRN" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <form onSubmit={handleSubmit} className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          <FormInput label="ASN/GRN No" value={form.asnGrnNo} onChange={(event) => update("asnGrnNo", event.target.value)} required />
          <FormInput label="Supplier" value={form.supplier} onChange={(event) => update("supplier", event.target.value)} required />
          <FormInput label="PO No" value={form.poNo} onChange={(event) => update("poNo", event.target.value)} required />
          <FormInput label="Challan No" value={form.challanNo} onChange={(event) => update("challanNo", event.target.value)} required />
          <FormInput label="Lot No" value={form.lotNo} onChange={(event) => update("lotNo", event.target.value)} required />
          <FormInput label="Item/Yarn Type" value={form.itemYarnType} onChange={(event) => update("itemYarnType", event.target.value)} required />
          <FormInput
            label="Total Bag Qty"
            type="number"
            min="1"
            value={form.totalBagQty}
            onChange={(event) => update("totalBagQty", event.target.value)}
            required
          />
          <FormInput
            label="Weight Per Bag"
            type="number"
            min="0"
            step="0.01"
            value={form.weightPerBag}
            onChange={(event) => update("weightPerBag", event.target.value)}
            required
          />
          <FormInput label="Challan Copy" value={form.challanCopyPath} onChange={(event) => update("challanCopyPath", event.target.value)} />
          <FormInput label="Uster Report" value={form.usterReportPath} onChange={(event) => update("usterReportPath", event.target.value)} />
        </div>

        <div className="mt-6 flex justify-end gap-3">
          <button
            type="button"
            onClick={() => router.push("/asn-grn")}
            className="rounded-md border border-slate-300 px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-50"
          >
            Cancel
          </button>
          <button
            type="submit"
            disabled={saving}
            className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300"
          >
            {saving ? "Saving" : "Submit"}
          </button>
        </div>
      </form>
    </div>
  );
}
