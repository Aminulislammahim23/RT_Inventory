import { extractArray } from "./api";
import type { AsnGrn } from "@/types/asn";
import type { AuthUser, LoginResponse, UserRole } from "@/types/auth";
import type { PickingList, Requisition, RequisitionPurpose } from "@/types/requisition";
import type { ReturnRecord } from "@/types/returns";
import type { GateWarning, IssueScanResult, IssueSession, ReceiveScanResult, ReceiveSession, RfidTag, RfidTagHistory } from "@/types/rfid";
import type { StockItem, StockTransaction } from "@/types/stock";

type ApiRecord = Record<string, unknown>;

const warningLabels: Record<string, string> = {
  AsnGrnNotScanned: "Missing ASN",
  ExtraBagDetected: "Extra Bag",
  MismatchedRfidTag: "Mismatch Tag",
  PickingListNotScanned: "Issue Warning",
  WrongBagScanned: "Mismatch Tag",
};

function asRecord(value: unknown): ApiRecord {
  return value && typeof value === "object" ? (value as ApiRecord) : {};
}

function text(value: unknown, fallback = "") {
  return typeof value === "string" ? value : value == null ? fallback : String(value);
}

function optionalText(value: unknown) {
  const result = text(value);
  return result ? result : undefined;
}

function numberValue(value: unknown, fallback = 0) {
  const result = typeof value === "number" ? value : Number(value);
  return Number.isFinite(result) ? result : fallback;
}

function idValue(value: unknown, fallback = "") {
  return typeof value === "number" || typeof value === "string" ? value : fallback;
}

function boolValue(value: unknown, fallback = false) {
  return typeof value === "boolean" ? value : fallback;
}

function arrayOf<T>(payload: unknown, normalize: (item: unknown) => T) {
  return extractArray<unknown>(payload, []).map(normalize);
}

export function normalizeUsers(payload: unknown) {
  return arrayOf(payload, normalizeUser);
}

export function normalizeAsnGrns(payload: unknown) {
  return arrayOf(payload, normalizeAsnGrn);
}

export function normalizeRfidTags(payload: unknown) {
  return arrayOf(payload, normalizeRfidTag);
}

export function normalizeRfidTagHistoryList(payload: unknown) {
  return arrayOf(payload, normalizeRfidTagHistory);
}

export function normalizeStockItems(payload: unknown) {
  return arrayOf(payload, normalizeStockItem);
}

export function normalizeStockTransactions(payload: unknown) {
  return arrayOf(payload, normalizeStockTransaction);
}

export function normalizeWarnings(payload: unknown) {
  return arrayOf(payload, normalizeGateWarning);
}

export function normalizeRequisitions(payload: unknown) {
  return arrayOf(payload, normalizeRequisition);
}

export function normalizeReturns(payload: unknown) {
  return arrayOf(payload, normalizeReturn);
}

export function normalizeAuthResponse(payload: unknown): LoginResponse {
  const record = asRecord(payload);
  return {
    token: text(record.token),
    expiresAt: optionalText(record.expiresAt),
    user: record.user ? normalizeUser(record.user) : undefined,
  };
}

export function normalizeUser(payload: unknown): AuthUser {
  const record = asRecord(payload);
  const isActive = typeof record.isActive === "boolean" ? record.isActive : text(record.status, "Active").toLowerCase() !== "inactive";
  const name = text(record.name ?? record.fullName ?? record.username);

  return {
    id: idValue(record.id),
    name,
    fullName: text(record.fullName ?? name),
    username: text(record.username),
    email: optionalText(record.email),
    role: text(record.role, "Admin") as UserRole,
    status: isActive ? "Active" : "Inactive",
    isActive,
    createdAt: optionalText(record.createdAt),
  };
}

export function normalizeAsnGrn(payload: unknown): AsnGrn {
  const record = asRecord(payload);
  const challanCopyPath = optionalText(record.challanCopyPath ?? record.challanCopy);
  const usterReportPath = optionalText(record.usterReportPath ?? record.usterReport);

  return {
    id: idValue(record.id),
    asnGrnNo: text(record.asnGrnNo),
    supplier: text(record.supplier),
    poNo: text(record.poNo),
    challanNo: text(record.challanNo),
    lotNo: text(record.lotNo),
    itemYarnType: text(record.itemYarnType),
    totalBagQty: numberValue(record.totalBagQty),
    weightPerBag: numberValue(record.weightPerBag),
    totalWeight: numberValue(record.totalWeight),
    challanCopy: challanCopyPath,
    usterReport: usterReportPath,
    challanCopyPath,
    usterReportPath,
    status: text(record.status, "Parking"),
    createdByUsername: optionalText(record.createdByUsername),
    createdAt: optionalText(record.createdAt),
    updatedAt: optionalText(record.updatedAt),
    assignedTags: Array.isArray(record.assignedTags) ? record.assignedTags.map(normalizeRfidTag) : undefined,
  };
}

export function normalizeRfidTag(payload: unknown): RfidTag {
  const record = asRecord(payload);
  const isActive = typeof record.isActive === "boolean" ? record.isActive : text(record.status, "Active").toLowerCase() !== "inactive";
  const tagValue = text(record.tagValue ?? record.tagNo);
  const bagNumber = numberValue(record.bagNumber ?? record.bagNo, 0);
  const assignmentType = optionalText(record.assignmentType);

  return {
    id: idValue(record.id),
    tagNo: tagValue,
    tagValue,
    epc: optionalText(record.epc),
    asnGrnId: idValue(record.asnGrnId, undefined),
    asnGrnNo: optionalText(record.asnGrnNo),
    bagNo: optionalText(record.bagNo) ?? (bagNumber ? String(bagNumber) : undefined),
    bagNumber: bagNumber || undefined,
    lotNo: optionalText(record.lotNo),
    itemYarnType: optionalText(record.itemYarnType),
    status: text(record.status, isActive ? "Active" : "Inactive"),
    currentStatus: optionalText(record.currentStatus ?? assignmentType),
    assignmentType,
    isActive,
    lastSeenAt: optionalText(record.lastSeenAt ?? record.updatedAt ?? record.createdAt),
    assignedAt: optionalText(record.assignedAt ?? record.updatedAt),
    createdAt: optionalText(record.createdAt),
    updatedAt: optionalText(record.updatedAt),
  };
}

export function normalizeRfidTagHistory(payload: unknown): RfidTagHistory {
  const record = asRecord(payload);
  const tagValue = text(record.tagValue ?? record.tagNo);

  return {
    id: idValue(record.id),
    rfidTagId: idValue(record.rfidTagId, undefined),
    tagNo: tagValue,
    tagValue,
    action: text(record.action),
    remarks: optionalText(record.remarks),
    referenceNo: optionalText(record.referenceNo ?? record.remarks),
    location: optionalText(record.location),
    timestamp: text(record.timestamp ?? record.createdAt),
    createdAt: optionalText(record.createdAt),
    user: optionalText(record.user ?? record.performedByUsername),
  };
}

export function normalizeStockItem(payload: unknown): StockItem {
  const record = asRecord(payload);
  const weight = numberValue(record.weight ?? record.weightPerBag);
  const bagQty = numberValue(record.bagQty ?? record.quantity, 1);

  return {
    id: idValue(record.id),
    asnGrnId: idValue(record.asnGrnId, undefined),
    asnGrnNo: text(record.asnGrnNo),
    rfidTagId: idValue(record.rfidTagId, undefined),
    tagValue: optionalText(record.tagValue ?? record.tagNo),
    lotNo: text(record.lotNo),
    itemYarnType: text(record.itemYarnType),
    supplier: text(record.supplier),
    bagQty,
    weightPerBag: numberValue(record.weightPerBag ?? weight),
    totalWeight: numberValue(record.totalWeight, weight * bagQty),
    status: text(record.status),
    lastMovementAt: optionalText(record.lastMovementAt ?? record.updatedAt ?? record.createdAt),
  };
}

export function normalizeStockTransaction(payload: unknown): StockTransaction {
  const record = asRecord(payload);
  const tagValue = text(record.tagValue ?? record.tagNo);

  return {
    id: idValue(record.id),
    referenceNo: text(record.referenceNo ?? record.asnGrnNo ?? record.requisitionId ?? "-"),
    tagNo: tagValue,
    tagValue,
    itemYarnType: text(record.itemYarnType),
    lotNo: text(record.lotNo),
    transactionType: text(record.transactionType),
    quantity: numberValue(record.quantity, 1),
    weight: numberValue(record.weight),
    createdAt: text(record.createdAt),
    createdBy: text(record.createdBy ?? record.createdByUsername),
  };
}

export function normalizeGateWarning(payload: unknown): GateWarning {
  const record = asRecord(payload);
  const rawType = text(record.type ?? record.warningType, "Warning");
  const type = warningLabels[rawType] ?? rawType;
  const referenceNo = optionalText(record.referenceNo ?? record.rfidTagValue ?? record.asnGrnId ?? record.requisitionId);

  return {
    id: idValue(record.id),
    type,
    message: text(record.message),
    severity: type.toLowerCase().includes("mismatch") || type.toLowerCase().includes("wrong") ? "danger" : "warning",
    referenceNo,
    createdAt: text(record.createdAt, new Date().toISOString()),
  };
}

export function normalizeReceiveSession(payload: unknown): ReceiveSession {
  const record = asRecord(payload);

  return {
    id: idValue(record.id),
    asnGrnId: idValue(record.asnGrnId, undefined),
    asnGrnNo: text(record.asnGrnNo),
    status: text(record.status, "Active"),
    startedAt: text(record.startedAt, new Date().toISOString()),
    lastActivityAt: optionalText(record.lastActivityAt),
    endedAt: optionalText(record.endedAt) ?? null,
    receivedTags: Array.isArray(record.receivedTags) ? record.receivedTags.map(normalizeRfidTag) : [],
    warnings: Array.isArray(record.warnings) ? record.warnings.map(normalizeGateWarning) : [],
  };
}

export function normalizeReceiveScan(payload: unknown): ReceiveScanResult {
  const record = asRecord(payload);
  return {
    isValid: boolValue(record.isValid),
    message: text(record.message),
    receiveSessionId: idValue(record.receiveSessionId),
    tagValue: text(record.tagValue),
    rfidTagId: idValue(record.rfidTagId, undefined),
    stockId: idValue(record.stockId, undefined),
    warningType: optionalText(record.warningType) ?? null,
  };
}

export function receiveScanToTag(scan: ReceiveScanResult): RfidTag {
  return {
    id: scan.rfidTagId ?? `${scan.receiveSessionId}-${scan.tagValue}`,
    tagNo: scan.tagValue,
    tagValue: scan.tagValue,
    status: "Active",
    currentStatus: "Parking",
    lastSeenAt: new Date().toISOString(),
  };
}

export function normalizeIssueSession(payload: unknown): IssueSession {
  const record = asRecord(payload);

  return {
    id: idValue(record.id),
    pickingListId: idValue(record.pickingListId, undefined),
    pickingListNo: text(record.pickingListNo),
    status: text(record.status, "Active"),
    startedAt: text(record.startedAt, new Date().toISOString()),
    lastActivityAt: optionalText(record.lastActivityAt),
    endedAt: optionalText(record.endedAt) ?? null,
    issuedTags: Array.isArray(record.issuedTags) ? record.issuedTags.map(normalizeRfidTag) : [],
    warnings: Array.isArray(record.warnings) ? record.warnings.map(normalizeGateWarning) : [],
  };
}

export function normalizeIssueScan(payload: unknown): IssueScanResult {
  const record = asRecord(payload);
  return {
    isValid: boolValue(record.isValid),
    message: text(record.message),
    issueSessionId: idValue(record.issueSessionId),
    tagValue: text(record.tagValue),
    rfidTagId: idValue(record.rfidTagId, undefined),
    stockId: idValue(record.stockId, undefined),
    warningType: optionalText(record.warningType) ?? null,
  };
}

export function issueScanToTag(scan: IssueScanResult): RfidTag {
  return {
    id: scan.rfidTagId ?? `${scan.issueSessionId}-${scan.tagValue}`,
    tagNo: scan.tagValue,
    tagValue: scan.tagValue,
    status: "Active",
    currentStatus: "Issued",
    lastSeenAt: new Date().toISOString(),
  };
}

export function scanToWarning(scan: ReceiveScanResult | IssueScanResult): GateWarning {
  return normalizeGateWarning({
    id: `${scan.tagValue}-${Date.now()}`,
    warningType: scan.warningType ?? "Warning",
    message: scan.message,
    rfidTagValue: scan.tagValue,
    createdAt: new Date().toISOString(),
  });
}

export function normalizePickingList(payload: unknown): PickingList {
  const record = asRecord(payload);
  return {
    id: idValue(record.id),
    requisitionId: idValue(record.requisitionId),
    pickingListNo: text(record.pickingListNo),
    barcodeValue: text(record.barcodeValue),
    createdByUsername: optionalText(record.createdByUsername),
    createdAt: optionalText(record.createdAt),
  };
}

export function normalizeRequisition(payload: unknown): Requisition {
  const record = asRecord(payload);

  return {
    id: idValue(record.id),
    requisitionNo: text(record.requisitionNo),
    purpose: text(record.purpose, "Knitting") as RequisitionPurpose,
    status: text(record.status, "Pending"),
    createdBy: text(record.createdBy ?? record.createdByUsername),
    date: text(record.date ?? record.createdAt),
    createdAt: optionalText(record.createdAt),
    updatedAt: optionalText(record.updatedAt),
    items: arrayOf(record.items, (item) => {
      const line = asRecord(item);
      const bagQty = numberValue(line.requiredBagQty ?? line.bagQty);
      return {
        id: idValue(line.id),
        itemYarnType: text(line.itemYarnType),
        lotNo: text(line.lotNo),
        bagQty,
        requiredBagQty: bagQty,
        requiredWeight: numberValue(line.requiredWeight),
      };
    }),
    pickingLists: arrayOf(record.pickingLists, normalizePickingList),
  };
}

export function normalizeReturn(payload: unknown): ReturnRecord {
  const record = asRecord(payload);
  const sourceRequisitionId = idValue(record.sourceRequisitionId, undefined);
  const asnGrnNo = text(record.asnGrnNo ?? record.returnAsnGrnNo);
  const rawStatus = text(record.status, asnGrnNo ? "AsnGrnCreated" : "Pending");

  return {
    id: idValue(record.id),
    returnNo: text(record.returnNo),
    source: text(record.source, sourceRequisitionId ? `Requisition #${sourceRequisitionId}` : "Manual Return"),
    sourceRequisitionId: sourceRequisitionId ?? null,
    returnAsnGrnId: idValue(record.returnAsnGrnId, undefined) ?? null,
    returnAsnGrnNo: optionalText(record.returnAsnGrnNo),
    asnGrnNo,
    lotNo: text(record.lotNo),
    itemYarnType: text(record.itemYarnType),
    bagQty: numberValue(record.bagQty ?? record.totalBagQty),
    weightPerBag: numberValue(record.weightPerBag),
    status: rawStatus === "AsnGrnCreated" ? "Return ASN Created" : rawStatus === "Pending" ? "Pending ASN" : rawStatus,
    date: text(record.date ?? record.createdAt),
    createdByUsername: optionalText(record.createdByUsername),
  };
}

export function cleanParams(params: Record<string, string>) {
  return Object.fromEntries(Object.entries(params).filter(([, value]) => value.trim()));
}
