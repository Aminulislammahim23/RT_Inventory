"use client";

import type { Key, ReactNode } from "react";

export type DataTableColumn<Row> = {
  header: string;
  accessor?: keyof Row;
  render?: (row: Row, index: number) => ReactNode;
  className?: string;
};

type DataTableProps<Row> = {
  columns: DataTableColumn<Row>[];
  data: Row[];
  emptyMessage?: string;
  rowKey?: (row: Row, index: number) => Key;
};

export function DataTable<Row extends object>({
  columns,
  data,
  emptyMessage = "No records found",
  rowKey,
}: DataTableProps<Row>) {
  return (
    <div className="overflow-hidden rounded-lg border border-slate-200 bg-white">
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-slate-200 text-sm">
          <thead className="bg-slate-50">
            <tr>
              {columns.map((column) => (
                <th
                  key={column.header}
                  className={`whitespace-nowrap px-4 py-3 text-left text-xs font-bold uppercase tracking-wide text-slate-500 ${column.className ?? ""}`}
                >
                  {column.header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 bg-white">
            {data.length ? (
              data.map((row, index) => (
                <tr key={rowKey?.(row, index) ?? index} className="hover:bg-slate-50">
                  {columns.map((column) => {
                    const value = column.render
                      ? column.render(row, index)
                      : column.accessor
                        ? String(row[column.accessor] ?? "")
                        : "";

                    return (
                      <td key={column.header} className={`whitespace-nowrap px-4 py-3 text-slate-700 ${column.className ?? ""}`}>
                        {value}
                      </td>
                    );
                  })}
                </tr>
              ))
            ) : (
              <tr>
                <td className="px-4 py-8 text-center text-sm font-medium text-slate-500" colSpan={columns.length}>
                  {emptyMessage}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
