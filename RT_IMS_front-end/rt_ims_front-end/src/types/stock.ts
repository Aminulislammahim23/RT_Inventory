import type { EntityId } from "./auth";

export type StockStatus = "Parking" | "Confirmed" | "Issued" | "Return";

export interface StockItem {
  id: EntityId;
  asnGrnId?: EntityId;
  asnGrnNo: string;
  rfidTagId?: EntityId;
  tagValue?: string;
  lotNo: string;
  itemYarnType: string;
  supplier: string;
  bagQty: number;
  weightPerBag: number;
  totalWeight: number;
  status: StockStatus | string;
  lastMovementAt?: string;
}

export interface StockTransaction {
  id: EntityId;
  referenceNo: string;
  tagNo: string;
  tagValue?: string;
  itemYarnType: string;
  lotNo: string;
  transactionType: "Receive" | "MRR Approved" | "Issue" | "Return" | string;
  quantity: number;
  weight: number;
  createdAt: string;
  createdBy: string;
}

export interface StockSummary {
  parkingStock: number;
  confirmedStock: number;
  issuedStock: number;
  returnStock: number;
  warningCount: number;
}
