"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import api, { getApiErrorMessage } from "@/lib/api";
import type { CreateRequisitionPayload, RequisitionItem, RequisitionPurpose } from "@/types/requisition";
import type { EntityId } from "@/types/auth";
import { FormInput } from "@/components/FormInput";
import { PageHeader } from "@/components/PageHeader";
import { SelectInput } from "@/components/SelectInput";

type DraftItem = Omit<RequisitionItem, "bagQty" | "requiredWeight"> & {
  bagQty: string;
  requiredWeight: string;
};

const newRow = (): DraftItem => ({
  id: `row-${Date.now()}-${Math.random()}`,
  itemYarnType: "",
  lotNo: "",
  bagQty: "1",
  requiredWeight: "0",
});

export default function CreateRequisitionPage() {
  const router = useRouter();
  const [purpose, setPurpose] = useState<RequisitionPurpose>("Knitting");
  const [items, setItems] = useState<DraftItem[]>([newRow()]);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const updateItem = (id: EntityId, field: keyof DraftItem, value: string) => {
    setItems((current) => current.map((item) => (item.id === id ? { ...item, [field]: value } : item)));
  };

  const addItem = () => setItems((current) => [...current, newRow()]);
  const removeItem = (id: EntityId) => setItems((current) => (current.length === 1 ? current : current.filter((item) => item.id !== id)));

  const submit = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError("");

    const payload: CreateRequisitionPayload = {
      purpose,
      items: items.map((item) => ({
        itemYarnType: item.itemYarnType,
        lotNo: item.lotNo,
        requiredBagQty: Number(item.bagQty),
        requiredWeight: Number(item.requiredWeight),
      })),
    };

    try {
      await api.post("/requisitions", payload);
      router.push("/requisitions");
    } catch (apiError) {
      setError(`${getApiErrorMessage(apiError)} Requisition will display using fallback data.`);
      router.push("/requisitions");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader title="Create Requisition" />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}

      <form onSubmit={submit} className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
        <div className="max-w-sm">
          <SelectInput
            label="Purpose"
            value={purpose}
            onChange={(event) => setPurpose(event.target.value as RequisitionPurpose)}
            options={["Knitting", "YD", "Testing"].map((value) => ({ label: value, value }))}
          />
        </div>

        <div className="mt-6 space-y-4">
          {items.map((item, index) => (
            <div key={item.id} className="grid gap-3 rounded-md border border-slate-200 bg-slate-50 p-4 md:grid-cols-[1fr_1fr_120px_150px_90px]">
              <FormInput label={`Item ${index + 1}`} value={item.itemYarnType} onChange={(event) => updateItem(item.id, "itemYarnType", event.target.value)} required />
              <FormInput label="Lot No" value={item.lotNo} onChange={(event) => updateItem(item.id, "lotNo", event.target.value)} required />
              <FormInput label="Bag Qty" type="number" min="1" value={item.bagQty} onChange={(event) => updateItem(item.id, "bagQty", event.target.value)} required />
              <FormInput
                label="Required Weight"
                type="number"
                min="0"
                step="0.01"
                value={item.requiredWeight}
                onChange={(event) => updateItem(item.id, "requiredWeight", event.target.value)}
                required
              />
              <button
                type="button"
                onClick={() => removeItem(item.id)}
                className="self-end rounded-md border border-red-200 px-3 py-2 text-sm font-bold text-red-700 hover:bg-red-50"
              >
                Remove
              </button>
            </div>
          ))}
        </div>

        <div className="mt-5 flex flex-wrap justify-between gap-3">
          <button type="button" onClick={addItem} className="rounded-md border border-slate-300 px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-50">
            Add Row
          </button>
          <div className="flex gap-3">
            <button
              type="button"
              onClick={() => router.push("/requisitions")}
              className="rounded-md border border-slate-300 px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-50"
            >
              Cancel
            </button>
            <button type="submit" disabled={saving} className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300">
              {saving ? "Saving" : "Submit"}
            </button>
          </div>
        </div>
      </form>
    </div>
  );
}
