import type { EntityId } from "./auth";

export type RfidTagStatus = "Active" | "Inactive" | "Assigned" | "Scanned";

export interface RfidTag {
  id: EntityId;
  tagNo: string;
  tagValue?: string;
  epc?: string;
  asnGrnId?: EntityId;
  asnGrnNo?: string;
  bagNo?: string;
  bagNumber?: number;
  lotNo?: string;
  itemYarnType?: string;
  status: RfidTagStatus | string;
  currentStatus?: string;
  assignmentType?: string;
  isActive?: boolean;
  lastSeenAt?: string;
  assignedAt?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface RfidTagHistory {
  id: EntityId;
  rfidTagId?: EntityId;
  tagNo: string;
  tagValue?: string;
  action: string;
  referenceNo?: string;
  location?: string;
  timestamp: string;
  user?: string;
  remarks?: string;
  createdAt?: string;
}

export interface GateWarning {
  id: EntityId;
  type: "Extra Bag" | "Mismatch Tag" | "Missing ASN" | "Duplicate Scan" | "Issue Warning" | string;
  message: string;
  severity: "warning" | "danger" | "info";
  referenceNo?: string;
  createdAt: string;
}

export interface ReceiveSession {
  id: EntityId;
  asnGrnId?: EntityId;
  asnGrnNo: string;
  status: "Active" | "Inactive" | "Deactivated" | "Expired" | string;
  startedAt: string;
  lastActivityAt?: string;
  endedAt?: string | null;
  receivedTags: RfidTag[];
  warnings: GateWarning[];
}

export interface IssueSession {
  id: EntityId;
  pickingListId?: EntityId;
  pickingListNo: string;
  status: "Active" | "Inactive" | "Deactivated" | "Expired" | string;
  startedAt: string;
  lastActivityAt?: string;
  endedAt?: string | null;
  issuedTags: RfidTag[];
  warnings: GateWarning[];
}

export interface ReceiveScanResult {
  isValid: boolean;
  message: string;
  receiveSessionId: EntityId;
  tagValue: string;
  rfidTagId?: EntityId;
  stockId?: EntityId;
  warningType?: string | null;
}

export interface IssueScanResult {
  isValid: boolean;
  message: string;
  issueSessionId: EntityId;
  tagValue: string;
  rfidTagId?: EntityId;
  stockId?: EntityId;
  warningType?: string | null;
}
