import type { EntityId } from "./auth";

export type RequisitionPurpose = "Knitting" | "YD" | "Testing";
export type RequisitionStatus = "Draft" | "Pending" | "Picked" | "Picking" | "Issued" | "Closed" | "Cancelled";

export interface RequisitionItem {
  id: EntityId;
  itemYarnType: string;
  lotNo: string;
  bagQty: number;
  requiredBagQty?: number;
  requiredWeight: number;
}

export interface PickingList {
  id: EntityId;
  requisitionId: EntityId;
  pickingListNo: string;
  barcodeValue: string;
  createdByUsername?: string;
  createdAt?: string;
}

export interface Requisition {
  id: EntityId;
  requisitionNo: string;
  purpose: RequisitionPurpose;
  status: RequisitionStatus | string;
  createdBy: string;
  date: string;
  items: RequisitionItem[];
  pickingLists?: PickingList[];
  createdAt?: string;
  updatedAt?: string;
}

export interface CreateRequisitionPayload {
  purpose: RequisitionPurpose;
  items: Array<{
    itemYarnType: string;
    lotNo?: string;
    requiredBagQty: number;
    requiredWeight: number;
  }>;
}
