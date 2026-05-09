import type { RfidTag } from "./rfid";
import type { EntityId } from "./auth";

export type AsnGrnStatus =
  | "Draft"
  | "Parking"
  | "Pending"
  | "Confirmed"
  | "Issued"
  | "Return"
  | "Rejected";

export interface AsnGrn {
  id: EntityId;
  asnGrnNo: string;
  supplier: string;
  poNo: string;
  challanNo: string;
  lotNo: string;
  itemYarnType: string;
  totalBagQty: number;
  weightPerBag: number;
  totalWeight?: number;
  challanCopy?: string;
  usterReport?: string;
  challanCopyPath?: string;
  usterReportPath?: string;
  status: AsnGrnStatus | string;
  createdByUsername?: string;
  createdAt?: string;
  updatedAt?: string;
  assignedTags?: RfidTag[];
}

export interface CreateAsnGrnPayload {
  asnGrnNo: string;
  supplier: string;
  poNo: string;
  challanNo: string;
  lotNo: string;
  itemYarnType: string;
  totalBagQty: number;
  weightPerBag: number;
  challanCopyPath?: string;
  usterReportPath?: string;
}
