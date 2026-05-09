"use client";

import { useEffect, useMemo, useState } from "react";
import api, { getApiErrorMessage, withFallback } from "@/lib/api";
import { normalizeUser, normalizeUsers } from "@/lib/api-contract";
import { fallbackUsers } from "@/lib/fallback-data";
import { formatDate } from "@/lib/format";
import type { AuthUser, CreateUserPayload, UserRole } from "@/types/auth";
import { DataTable, type DataTableColumn } from "@/components/DataTable";
import { FormInput } from "@/components/FormInput";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { Modal } from "@/components/Modal";
import { PageHeader } from "@/components/PageHeader";
import { SelectInput } from "@/components/SelectInput";
import { StatusBadge } from "@/components/StatusBadge";

const roles: UserRole[] = [
  "SCM",
  "Store Officer",
  "Store Supervisor",
  "Store Manager",
  "Loader",
  "Knitting Supervisor",
  "Unit Planner",
  "Quality Officer",
  "Admin",
];

const initialForm: CreateUserPayload = {
  name: "",
  username: "",
  email: "",
  password: "",
  role: "Store Officer",
};

export default function UsersPage() {
  const [users, setUsers] = useState<AuthUser[]>(fallbackUsers);
  const [form, setForm] = useState<CreateUserPayload>(initialForm);
  const [modalOpen, setModalOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    const load = async () => {
      const result = await withFallback<AuthUser[]>(
        () => api.get("/users"),
        fallbackUsers,
        normalizeUsers,
      );
      setUsers(result.data);
      setError(result.error);
      setLoading(false);
    };

    load();
  }, []);

  const createUser = async (event: React.FormEvent) => {
    event.preventDefault();
    setSaving(true);
    setError("");

    const fallbackUser: AuthUser = {
      id: `user-${Date.now()}`,
      ...form,
      status: "Active",
      createdAt: new Date().toISOString(),
    };

    try {
      const response = await api.post("/users", {
        username: form.username,
        fullName: form.name,
        email: form.email,
        password: form.password,
        role: form.role,
        isActive: true,
      });
      setUsers((current) => [normalizeUser(response.data), ...current]);
      setForm(initialForm);
      setModalOpen(false);
    } catch (apiError) {
      setUsers((current) => [fallbackUser, ...current]);
      setError(getApiErrorMessage(apiError));
      setForm(initialForm);
      setModalOpen(false);
    } finally {
      setSaving(false);
    }
  };

  const toggleStatus = async (user: AuthUser) => {
    const nextStatus = user.status === "Active" ? "Inactive" : "Active";
    setUsers((current) => current.map((item) => (item.id === user.id ? { ...item, status: nextStatus } : item)));

    try {
      const response = await api.patch(`/users/${user.id}/status`, { isActive: nextStatus === "Active" });
      setUsers((current) => current.map((item) => (item.id === user.id ? normalizeUser(response.data) : item)));
    } catch (apiError) {
      setError(getApiErrorMessage(apiError));
    }
  };

  const columns = useMemo<DataTableColumn<AuthUser>[]>(
    () => [
      { header: "Name", accessor: "name" },
      { header: "Username", accessor: "username" },
      { header: "Email", render: (row) => row.email ?? "-" },
      { header: "Role", accessor: "role" },
      { header: "Status", render: (row) => <StatusBadge status={row.status} /> },
      { header: "Created", render: (row) => formatDate(row.createdAt) },
      {
        header: "Actions",
        render: (row) => (
          <button type="button" onClick={() => toggleStatus(row)} className="font-bold text-blue-700 hover:text-blue-900">
            {row.status === "Active" ? "Deactivate" : "Activate"}
          </button>
        ),
      },
    ],
    [],
  );

  return (
    <div>
      <PageHeader
        title="User Management"
        actions={
          <button type="button" onClick={() => setModalOpen(true)} className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700">
            Create User
          </button>
        }
      />

      {error ? <div className="mb-5 rounded-md border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">{error}</div> : null}
      {loading ? <LoadingSpinner /> : <DataTable columns={columns} data={users} rowKey={(row) => row.id} />}

      <Modal
        isOpen={modalOpen}
        title="Create User"
        onClose={() => setModalOpen(false)}
        footer={
          <>
            <button
              type="button"
              onClick={() => setModalOpen(false)}
              className="rounded-md border border-slate-300 px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              form="create-user-form"
              disabled={saving}
              className="rounded-md bg-blue-600 px-4 py-2 text-sm font-bold text-white hover:bg-blue-700 disabled:bg-blue-300"
            >
              {saving ? "Saving" : "Create"}
            </button>
          </>
        }
      >
        <form id="create-user-form" onSubmit={createUser} className="grid gap-4 md:grid-cols-2">
          <FormInput label="Name" value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} required />
          <FormInput label="Username" value={form.username} onChange={(event) => setForm({ ...form, username: event.target.value })} required />
          <FormInput label="Email" type="email" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} required />
          <FormInput label="Password" type="password" value={form.password} onChange={(event) => setForm({ ...form, password: event.target.value })} required />
          <SelectInput
            label="Role"
            value={form.role}
            onChange={(event) => setForm({ ...form, role: event.target.value as UserRole })}
            options={roles.map((role) => ({ label: role, value: role }))}
          />
        </form>
      </Modal>
    </div>
  );
}
