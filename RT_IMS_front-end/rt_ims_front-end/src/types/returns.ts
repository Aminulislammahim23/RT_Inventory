import type { EntityId } from "./auth";

export interface ReturnRecord {
  id: EntityId;
  returnNo: string;
  source: string;
  sourceRequisitionId?: EntityId | null;
  returnAsnGrnId?: EntityId | null;
  returnAsnGrnNo?: string | null;
  asnGrnNo: string;
  lotNo: string;
  itemYarnType: string;
  bagQty: number;
  weightPerBag?: number;
  status: string;
  date: string;
  createdByUsername?: string;
}
