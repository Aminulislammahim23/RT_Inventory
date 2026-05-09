namespace RT_Inventory.Api.Models.Enums;

public enum AsnGrnStatus
{
    Parking = 1,
    Confirmed = 2
}

public enum StockStatus
{
    Parking = 1,
    Confirmed = 2,
    Issued = 3,
    Return = 4
}

public enum RfidTagAssignmentType
{
    AsnGrnBag = 1,
    LooseBag = 2
}

public enum GateSessionStatus
{
    Active = 1,
    Deactivated = 2,
    Expired = 3
}

public enum WarningType
{
    AsnGrnNotScanned = 1,
    ExtraBagDetected = 2,
    MismatchedRfidTag = 3,
    PickingListNotScanned = 4,
    WrongBagScanned = 5
}

public enum StockTransactionType
{
    Receive = 1,
    Approval = 2,
    Issue = 3,
    Return = 4,
    Adjustment = 5
}

public enum RequisitionStatus
{
    Pending = 1,
    Picked = 2,
    Issued = 3,
    Cancelled = 4
}

public enum RequisitionPurpose
{
    Knitting = 1,
    YD = 2,
    Testing = 3
}

public enum ReturnRequestStatus
{
    Pending = 1,
    AsnGrnCreated = 2,
    Received = 3,
    Cancelled = 4
}
