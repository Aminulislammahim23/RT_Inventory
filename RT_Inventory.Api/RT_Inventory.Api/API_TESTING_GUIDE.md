# API Testing Guide

## Global Setup

1. Run the API in Development mode.
2. Open Swagger at `/swagger`.
3. Login using the default admin account:

```json
{
  "username": "admin",
  "password": "Admin@12345"
}
```

4. Copy the `token` value from the login response.
5. Click Swagger `Authorize` and enter:

```text
Bearer {token}
```

## Module: User/Login

### Endpoint: Login

- HTTP method: `POST`
- Endpoint: `/api/auth/login`
- Authorization role required: None

Request body example:

```json
{
  "username": "admin",
  "password": "Admin@12345"
}
```

Success response example:

```json
{
  "token": "jwt-token-value",
  "expiresAt": "2026-05-09T03:30:00Z",
  "user": {
    "id": 1,
    "username": "admin",
    "fullName": "System Administrator",
    "email": "admin@rt-inventory.local",
    "role": "Admin",
    "isActive": true,
    "createdAt": "2026-05-09T02:45:00Z"
  }
}
```

Error response example:

```json
{
  "message": "Invalid username or password."
}
```

Testing steps in Swagger/Postman:
1. Call `POST /api/auth/login`.
2. Verify HTTP `200 OK`.
3. Copy the JWT token.
4. Use the token to authorize protected endpoints.

Positive test cases:
- Login with seeded admin credentials returns token.
- Login response includes user role.

Negative test cases:
- Invalid password returns `401 Unauthorized`.
- Inactive user returns `401 Unauthorized`.
- Empty username or password returns `400 Bad Request`.

### Endpoint: Register User

- HTTP method: `POST`
- Endpoint: `/api/users`
- Authorization role required: `Admin`

Request body example:

```json
{
  "username": "store.officer",
  "fullName": "Store Officer",
  "email": "store.officer@example.com",
  "password": "Store@12345",
  "role": "Store Officer",
  "isActive": true
}
```

Success response example:

```json
{
  "id": 2,
  "username": "store.officer",
  "fullName": "Store Officer",
  "email": "store.officer@example.com",
  "role": "Store Officer",
  "isActive": true,
  "createdAt": "2026-05-09T02:55:00Z"
}
```

Error response example:

```json
{
  "message": "Username already exists."
}
```

Testing steps in Swagger/Postman:
1. Login as Admin.
2. Authorize Swagger/Postman with the token.
3. Call `POST /api/users`.
4. Verify HTTP `201 Created`.

Positive test cases:
- Admin can create a user with a valid role.
- Password is accepted when it meets length and complexity expectations.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Non-admin token returns `403 Forbidden`.
- Duplicate username returns `409 Conflict`.
- Unknown role returns `400 Bad Request`.

### Endpoint: List Users

- HTTP method: `GET`
- Endpoint: `/api/users`
- Authorization role required: `Admin`

Request body example: None

Success response example:

```json
[
  {
    "id": 1,
    "username": "admin",
    "fullName": "System Administrator",
    "email": "admin@rt-inventory.local",
    "role": "Admin",
    "isActive": true,
    "createdAt": "2026-05-09T02:45:00Z"
  }
]
```

Error response example:

```json
{
  "message": "Unauthorized"
}
```

Testing steps in Swagger/Postman:
1. Login as Admin.
2. Authorize with token.
3. Call `GET /api/users`.
4. Verify the response contains seeded admin and created users.

Positive test cases:
- Admin receives user list.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Non-admin token returns `403 Forbidden`.

### Endpoint: Get User Details

- HTTP method: `GET`
- Endpoint: `/api/users/{id}`
- Authorization role required: `Admin`

Request body example: None

Success response example:

```json
{
  "id": 1,
  "username": "admin",
  "fullName": "System Administrator",
  "email": "admin@rt-inventory.local",
  "role": "Admin",
  "isActive": true,
  "createdAt": "2026-05-09T02:45:00Z"
}
```

Error response example:

```json
{
  "message": "User was not found."
}
```

Testing steps in Swagger/Postman:
1. Login as Admin.
2. Authorize with token.
3. Call `GET /api/users/1`.
4. Verify HTTP `200 OK`.

Positive test cases:
- Existing user id returns user details.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Non-existing user id returns `404 Not Found`.

### Endpoint: Change User Active Status

- HTTP method: `PATCH`
- Endpoint: `/api/users/{id}/status`
- Authorization role required: `Admin`

Request body example:

```json
{
  "isActive": false
}
```

Success response example:

```json
{
  "id": 2,
  "username": "store.officer",
  "fullName": "Store Officer",
  "email": "store.officer@example.com",
  "role": "Store Officer",
  "isActive": false,
  "createdAt": "2026-05-09T02:55:00Z"
}
```

Error response example:

```json
{
  "message": "User was not found."
}
```

Testing steps in Swagger/Postman:
1. Login as Admin.
2. Create a test user.
3. Call `PATCH /api/users/{id}/status`.
4. Try login as the inactive user and verify login is rejected.

Positive test cases:
- Admin can deactivate a user.
- Admin can reactivate a user.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Non-admin token returns `403 Forbidden`.
- Non-existing user id returns `404 Not Found`.

## Pending Modules

The following modules are planned but not implemented yet:
- None for the initial requested module list.

## Module: Report

### Endpoints

- `GET /api/reports/current-stock?itemType=30/1&lot=LOT-A100&asn=GRN`
- `GET /api/reports/parking-stock`
- `GET /api/reports/confirmed-stock`
- `GET /api/reports/issued-stock`
- `GET /api/reports/asn-wise-receive?fromDate=2026-05-01&toDate=2026-05-31&supplier=ABC&lot=LOT&itemType=Cotton&asn=GRN`
- `GET /api/reports/requisition-wise-issue?fromDate=2026-05-01&toDate=2026-05-31&requisition=REQ&rfidTag=RFID`
- `GET /api/reports/rfid-tag-history?rfidTag=RFID`
- `GET /api/reports/warnings?fromDate=2026-05-01&toDate=2026-05-31&asn=GRN&requisition=REQ&rfidTag=RFID`

Authorization role required:
`Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Unit Planner`, `Knitting Supervisor`, `Quality Officer`

Request body example: None

Success response example for warning report:

```json
[
  {
    "id": 1,
    "warningType": "MismatchedRfidTag",
    "message": "RFID tag belongs to another ASN/GRN.",
    "rfidTagValue": "RFID-OTHER-001",
    "asnGrnId": 1,
    "requisitionId": null,
    "receiveSessionId": 1,
    "issueSessionId": null,
    "createdAt": "2026-05-09T06:00:00Z"
  }
]
```

Error response example:

```json
{
  "message": "Unauthorized"
}
```

Testing steps in Swagger/Postman:
1. Complete receive, approval, issue, and warning scenarios.
2. Call each report endpoint.
3. Apply filters one by one.
4. Verify filtered results match database state.

Positive test cases:
- Current stock returns all stock with optional item, lot, and ASN filters.
- Status-specific stock reports return only that status.
- ASN-wise receive groups received bags by ASN.
- Requisition-wise issue groups issued bags by requisition.
- RFID tag history filters by tag value.
- Warning report filters by date, ASN, requisition, and RFID tag.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.
- Filters with no matching data return an empty array.

## Module: Return

### Endpoint: Create Return Request

- HTTP method: `POST`
- Endpoint: `/api/returns`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Knitting Supervisor`

Request body example:

```json
{
  "sourceRequisitionId": 1
}
```

Success response example:

```json
{
  "id": 1,
  "returnNo": "RET-20260509-0001",
  "sourceRequisitionId": 1,
  "returnAsnGrnId": null,
  "returnAsnGrnNo": null,
  "status": "Pending",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T05:45:00Z"
}
```

Error response example:

```json
{
  "message": "Source requisition was not found."
}
```

### Endpoint: Create Return ASN/GRN

- HTTP method: `POST`
- Endpoint: `/api/returns/{id}/asn-grn`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Knitting Supervisor`

Request body example:

```json
{
  "lotNo": "LOT-RETURN-100",
  "itemYarnType": "30/1 Cotton",
  "totalBagQty": 2,
  "weightPerBag": 25.5
}
```

Success response example:

```json
{
  "id": 1,
  "returnNo": "RET-20260509-0001",
  "sourceRequisitionId": 1,
  "returnAsnGrnId": 10,
  "returnAsnGrnNo": "RTN-GRN-20260509054530",
  "status": "AsnGrnCreated",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T05:45:00Z"
}
```

Error response example:

```json
{
  "message": "Return ASN/GRN is already created."
}
```

Testing steps in Swagger/Postman:
1. Create return request.
2. Create return ASN/GRN.
3. Use RFID Tag and RFID Gate Receive flow against the return ASN/GRN.

Positive test cases:
- Return request number is generated.
- Return ASN/GRN is created in Parking status.

Negative test cases:
- Unknown source requisition returns `404 Not Found`.
- Duplicate return ASN/GRN creation returns `409 Conflict`.

## Module: RFID Gate Issue

### Endpoint: Start Issue Session

- HTTP method: `POST`
- Endpoint: `/api/issue-sessions`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example:

```json
{
  "pickingListId": 1
}
```

Success response example:

```json
{
  "id": 1,
  "pickingListId": 1,
  "pickingListNo": "PL-20260509-0001",
  "status": "Active",
  "startedByUserId": 1,
  "startedByUsername": "admin",
  "startedAt": "2026-05-09T05:30:00Z",
  "lastActivityAt": "2026-05-09T05:30:00Z",
  "endedAt": null
}
```

Error response example:

```json
{
  "message": "Picking list was not found."
}
```

Testing steps in Swagger/Postman:
1. Generate picking list.
2. Call `POST /api/issue-sessions`.
3. Verify status is `Active`.

Positive test cases:
- Existing picking list starts issue mode.

Negative test cases:
- Unknown picking list returns `404 Not Found`.

### Endpoint: Scan RFID Tag For Issue

- HTTP method: `POST`
- Endpoint: `/api/issue-sessions/{id}/scan`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example:

```json
{
  "tagValue": "RFID-000001"
}
```

Success response example:

```json
{
  "isValid": true,
  "message": "RFID tag issued and stock moved to Issued Stock.",
  "issueSessionId": 1,
  "tagValue": "RFID-000001",
  "rfidTagId": 1,
  "stockId": 1,
  "warningType": null
}
```

Error/warning response example:

```json
{
  "isValid": false,
  "message": "RFID tag stock does not match picking list requirements.",
  "issueSessionId": 1,
  "tagValue": "RFID-WRONG-001",
  "rfidTagId": 2,
  "stockId": null,
  "warningType": "WrongBagScanned"
}
```

Testing steps in Swagger/Postman:
1. Confirm stock through MRR approval.
2. Create requisition and picking list for matching item/lot.
3. Start issue session.
4. Scan matching RFID tag.
5. Verify stock moves from `Confirmed` to `Issued`.

Positive test cases:
- Matching confirmed stock is issued.
- Issue stock transaction is saved.

Negative test cases:
- Unknown tag returns warning.
- Non-confirmed stock returns warning.
- Item/lot mismatch returns warning.
- Expired session returns `400 Bad Request`.

### Endpoint: Deactivate Issue Session

- HTTP method: `POST`
- Endpoint: `/api/issue-sessions/{id}/deactivate`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example: None

Success response example:

```json
{
  "id": 1,
  "pickingListId": 1,
  "pickingListNo": "PL-20260509-0001",
  "status": "Deactivated",
  "startedByUserId": 1,
  "startedByUsername": "admin",
  "startedAt": "2026-05-09T05:30:00Z",
  "lastActivityAt": "2026-05-09T05:32:00Z",
  "endedAt": "2026-05-09T05:35:00Z"
}
```

Error response example:

```json
{
  "message": "Issue session was not found."
}
```

## Module: Requisition / Picking List

### Endpoint: Create Requisition

- HTTP method: `POST`
- Endpoint: `/api/requisitions`
- Authorization role required: `Admin`, `Knitting Supervisor`, `Unit Planner`, `Quality Officer`, `Store Officer`, `Store Supervisor`, `Store Manager`

Request body example:

```json
{
  "purpose": "Knitting",
  "items": [
    {
      "itemYarnType": "30/1 Cotton",
      "lotNo": "LOT-A100",
      "requiredBagQty": 5,
      "requiredWeight": 127.5
    }
  ]
}
```

Success response example:

```json
{
  "id": 1,
  "requisitionNo": "REQ-20260509-0001",
  "purpose": "Knitting",
  "status": "Pending",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T05:15:00Z",
  "updatedAt": null,
  "items": [],
  "pickingLists": []
}
```

Error response example:

```json
{
  "message": "Requisition purpose must be Knitting, YD, or Testing."
}
```

Testing steps in Swagger/Postman:
1. Login as an allowed role.
2. Call `POST /api/requisitions`.
3. Verify status is `Pending`.

Positive test cases:
- Valid purpose and item lines create requisition.
- Requisition number is generated.

Negative test cases:
- Invalid purpose returns `400 Bad Request`.
- Empty items returns `400 Bad Request`.

### Endpoint: Get Requisition Details

- HTTP method: `GET`
- Endpoint: `/api/requisitions/{id}`
- Authorization role required: `Admin`, `Knitting Supervisor`, `Unit Planner`, `Quality Officer`, `Store Officer`, `Store Supervisor`, `Store Manager`

Request body example: None

Success response example:

```json
{
  "id": 1,
  "requisitionNo": "REQ-20260509-0001",
  "purpose": "Knitting",
  "status": "Pending",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T05:15:00Z",
  "updatedAt": null,
  "items": [
    {
      "id": 1,
      "itemYarnType": "30/1 Cotton",
      "lotNo": "LOT-A100",
      "requiredBagQty": 5,
      "requiredWeight": 127.5
    }
  ],
  "pickingLists": []
}
```

Error response example:

```json
{
  "message": "Requisition was not found."
}
```

Testing steps in Swagger/Postman:
1. Create requisition.
2. Call `GET /api/requisitions/{id}`.
3. Verify item lines appear.

Positive test cases:
- Existing requisition returns details.

Negative test cases:
- Unknown id returns `404 Not Found`.

### Endpoint: Generate Picking List

- HTTP method: `POST`
- Endpoint: `/api/requisitions/{id}/picking-list`
- Authorization role required: `Admin`, `Knitting Supervisor`, `Unit Planner`, `Quality Officer`, `Store Officer`, `Store Supervisor`, `Store Manager`

Request body example: None

Success response example:

```json
{
  "id": 1,
  "requisitionId": 1,
  "pickingListNo": "PL-20260509-0001",
  "barcodeValue": "PL|REQ-20260509-0001|barcode-guid-value",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T05:20:00Z"
}
```

Error response example:

```json
{
  "message": "Requisition was not found."
}
```

Testing steps in Swagger/Postman:
1. Create requisition.
2. Call `POST /api/requisitions/{id}/picking-list`.
3. Verify barcode value is returned.
4. Fetch requisition details and verify status becomes `Picked`.

Positive test cases:
- Pending requisition generates picking list.
- Picking list barcode value is generated.

Negative test cases:
- Unknown requisition returns `404 Not Found`.
- Cancelled or issued requisition returns `400 Bad Request`.

## Module: MRR Approval

### Endpoint: Approve GRN And Generate MRR

- HTTP method: `POST`
- Endpoint: `/api/mrr-approvals/{asnGrnId}/approve`
- Authorization role required: `Admin`, `Store Manager`

Request body example: None

Success response example:

```json
{
  "id": 1,
  "asnGrnId": 1,
  "asnGrnNo": "GRN-2026-0001",
  "mrrNo": "MRR-20260509-0001",
  "approvedByUserId": 1,
  "approvedByUsername": "admin",
  "approvedAt": "2026-05-09T05:00:00Z",
  "confirmedBagQty": 10,
  "confirmedWeight": 255
}
```

Error response example:

```json
{
  "message": "ASN/GRN is already approved."
}
```

Testing steps in Swagger/Postman:
1. Create ASN/GRN.
2. Assign RFID tags to ASN/GRN bags.
3. Start receive session and scan valid tags.
4. Login as `Admin` or `Store Manager`.
5. Call `POST /api/mrr-approvals/{asnGrnId}/approve`.
6. Verify Parking Stock becomes Confirmed Stock.

Positive test cases:
- Store Manager can approve ASN/GRN with Parking Stock.
- MRR number is generated.
- ASN/GRN status becomes `Confirmed`.
- Stock transactions are saved with `Approval` type.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.
- Unknown ASN/GRN returns `404 Not Found`.
- Duplicate approval returns `409 Conflict`.
- ASN/GRN without Parking Stock returns `400 Bad Request`.

## Module: Stock

### Endpoint: Current Stock

- HTTP method: `GET`
- Endpoint: `/api/stocks/current?status=Parking`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Unit Planner`, `Knitting Supervisor`, `Quality Officer`

Request body example: None

Success response example:

```json
[
  {
    "id": 1,
    "asnGrnId": 1,
    "asnGrnNo": "GRN-2026-0001",
    "rfidTagId": 1,
    "tagValue": "RFID-000001",
    "lotNo": "LOT-A100",
    "itemYarnType": "30/1 Cotton",
    "weight": 25.5,
    "status": "Parking",
    "createdAt": "2026-05-09T04:30:00Z",
    "updatedAt": null
  }
]
```

Error response example:

```json
{
  "message": "Unauthorized"
}
```

Testing steps in Swagger/Postman:
1. Receive a valid RFID tag through a receive session.
2. Call `GET /api/stocks/current`.
3. Optionally filter with `status=Parking`, `Confirmed`, `Issued`, or `Return`.

Positive test cases:
- Current stock returns received bags.
- Status filter limits stock rows.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.

### Endpoint: Stock By Item/Yarn Type

- HTTP method: `GET`
- Endpoint: `/api/stocks/by-item?itemYarnType=30/1`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Unit Planner`, `Knitting Supervisor`, `Quality Officer`

Request body example: None

Success response example:

```json
[
  {
    "groupKey": "30/1 Cotton",
    "status": "Parking",
    "bagQty": 10,
    "totalWeight": 255
  }
]
```

Error response example:

```json
{
  "message": "Forbidden"
}
```

Testing steps in Swagger/Postman:
1. Receive stock for one or more yarn types.
2. Call `GET /api/stocks/by-item`.
3. Use `itemYarnType` query filter when needed.

Positive test cases:
- Groups stock by item/yarn type and status.
- Filter returns partial matches.

Negative test cases:
- Unsupported role returns `403 Forbidden`.

### Endpoint: Stock By Lot

- HTTP method: `GET`
- Endpoint: `/api/stocks/by-lot?lotNo=LOT-A100`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Unit Planner`, `Knitting Supervisor`, `Quality Officer`

Request body example: None

Success response example:

```json
[
  {
    "groupKey": "LOT-A100",
    "status": "Parking",
    "bagQty": 10,
    "totalWeight": 255
  }
]
```

Error response example:

```json
{
  "message": "Unauthorized"
}
```

Testing steps in Swagger/Postman:
1. Receive stock for one or more lots.
2. Call `GET /api/stocks/by-lot`.
3. Use `lotNo` query filter when needed.

Positive test cases:
- Groups stock by lot and status.
- Filter returns partial matches.

Negative test cases:
- Missing token returns `401 Unauthorized`.

### Endpoint: Stock Transaction History

- HTTP method: `GET`
- Endpoint: `/api/stocks/transactions?fromDate=2026-05-01&toDate=2026-05-31&rfidTag=RFID`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Unit Planner`, `Knitting Supervisor`, `Quality Officer`

Request body example: None

Success response example:

```json
[
  {
    "id": 1,
    "rfidTagId": 1,
    "tagValue": "RFID-000001",
    "asnGrnId": 1,
    "asnGrnNo": "GRN-2026-0001",
    "requisitionId": null,
    "transactionType": "Receive",
    "fromStatus": "Parking",
    "toStatus": "Parking",
    "weight": 25.5,
    "createdByUserId": 1,
    "createdByUsername": "admin",
    "createdAt": "2026-05-09T04:30:00Z"
  }
]
```

Error response example:

```json
{
  "message": "Forbidden"
}
```

Testing steps in Swagger/Postman:
1. Receive, approve, issue, or return stock.
2. Call `GET /api/stocks/transactions`.
3. Apply date or RFID tag filters.

Positive test cases:
- Receive operation creates transaction history.
- Date range filters transactions.
- RFID tag filter returns matching tag transactions.

Negative test cases:
- Unsupported role returns `403 Forbidden`.

## Module: RFID Gate Receive

### Endpoint: Start Receive Session

- HTTP method: `POST`
- Endpoint: `/api/receive-sessions`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example:

```json
{
  "asnGrnId": 1
}
```

Success response example:

```json
{
  "id": 1,
  "asnGrnId": 1,
  "asnGrnNo": "GRN-2026-0001",
  "status": "Active",
  "startedByUserId": 1,
  "startedByUsername": "admin",
  "startedAt": "2026-05-09T04:20:00Z",
  "lastActivityAt": "2026-05-09T04:20:00Z",
  "endedAt": null
}
```

Error response example:

```json
{
  "message": "ASN/GRN was not found."
}
```

Testing steps in Swagger/Postman:
1. Login as an allowed role.
2. Create an ASN/GRN.
3. Authorize with bearer token.
4. Call `POST /api/receive-sessions`.
5. Verify session status is `Active`.

Positive test cases:
- Existing ASN/GRN starts receive mode.
- Start user and start time are saved.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.
- Unknown ASN/GRN returns `404 Not Found` and warning log is saved.

### Endpoint: Scan RFID Tag In Receive Session

- HTTP method: `POST`
- Endpoint: `/api/receive-sessions/{id}/scan`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example:

```json
{
  "tagValue": "RFID-000001"
}
```

Success response example for valid tag:

```json
{
  "isValid": true,
  "message": "RFID tag received and Parking Stock updated.",
  "receiveSessionId": 1,
  "tagValue": "RFID-000001",
  "rfidTagId": 1,
  "stockId": 1,
  "warningType": null
}
```

Success response example for warning:

```json
{
  "isValid": false,
  "message": "RFID tag belongs to another ASN/GRN.",
  "receiveSessionId": 1,
  "tagValue": "RFID-OTHER-001",
  "rfidTagId": 2,
  "stockId": null,
  "warningType": "MismatchedRfidTag"
}
```

Error response example:

```json
{
  "message": "Receive session is not active or has expired."
}
```

Testing steps in Swagger/Postman:
1. Create ASN/GRN.
2. Assign RFID tag to that ASN/GRN bag.
3. Start receive session.
4. Call `POST /api/receive-sessions/{id}/scan`.
5. Verify valid tags create/update Parking Stock.

Positive test cases:
- Valid tag assigned to same ASN/GRN updates Parking Stock.
- Re-scanning valid tag keeps stock in Parking status.

Negative test cases:
- Unknown session returns `404 Not Found`.
- Expired inactive session returns `400 Bad Request`.
- Unknown tag returns warning response with `MismatchedRfidTag`.
- Loose tag returns warning response with `ExtraBagDetected`.
- Tag assigned to another ASN/GRN returns warning response with `MismatchedRfidTag`.

### Endpoint: Deactivate Receive Session

- HTTP method: `POST`
- Endpoint: `/api/receive-sessions/{id}/deactivate`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example: None

Success response example:

```json
{
  "id": 1,
  "asnGrnId": 1,
  "asnGrnNo": "GRN-2026-0001",
  "status": "Deactivated",
  "startedByUserId": 1,
  "startedByUsername": "admin",
  "startedAt": "2026-05-09T04:20:00Z",
  "lastActivityAt": "2026-05-09T04:22:00Z",
  "endedAt": "2026-05-09T04:25:00Z"
}
```

Error response example:

```json
{
  "message": "Receive session was not found."
}
```

Testing steps in Swagger/Postman:
1. Start a receive session.
2. Call `POST /api/receive-sessions/{id}/deactivate`.
3. Try scanning again and verify receive session is rejected.

Positive test cases:
- Active session can be deactivated.

Negative test cases:
- Unknown session returns `404 Not Found`.
- Scan after deactivation returns `400 Bad Request`.

## Module: RFID Tag

### Endpoint: RFID Tag Scan Entry

- HTTP method: `POST`
- Endpoint: `/api/rfid-tags/scan`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example:

```json
{
  "tagValue": "RFID-000001",
  "remarks": "Initial handheld scan"
}
```

Success response example:

```json
{
  "id": 1,
  "tagValue": "RFID-000001",
  "asnGrnId": null,
  "asnGrnNo": null,
  "bagNumber": null,
  "assignmentType": "LooseBag",
  "isActive": true,
  "createdAt": "2026-05-09T04:00:00Z",
  "updatedAt": null
}
```

Error response example:

```json
{
  "message": "Active RFID tag already exists."
}
```

Testing steps in Swagger/Postman:
1. Login as an allowed role.
2. Authorize with bearer token.
3. Call `POST /api/rfid-tags/scan`.
4. Verify HTTP `201 Created`.

Positive test cases:
- New RFID value creates an active tag.
- Tag history receives a `Scanned` record.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.
- Duplicate active tag value returns `409 Conflict`.
- Empty tag value returns `400 Bad Request`.

### Endpoint: Assign RFID Tag

- HTTP method: `POST`
- Endpoint: `/api/rfid-tags/assign`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example for ASN/GRN bag:

```json
{
  "tagValue": "RFID-000001",
  "asnGrnId": 1,
  "bagNumber": 1,
  "isLooseBag": false,
  "remarks": "Assigned to ASN bag 1"
}
```

Request body example for loose bag:

```json
{
  "tagValue": "RFID-LOOSE-001",
  "asnGrnId": null,
  "bagNumber": null,
  "isLooseBag": true,
  "remarks": "Loose bag tag"
}
```

Success response example:

```json
{
  "id": 1,
  "tagValue": "RFID-000001",
  "asnGrnId": 1,
  "asnGrnNo": "GRN-2026-0001",
  "bagNumber": 1,
  "assignmentType": "AsnGrnBag",
  "isActive": true,
  "createdAt": "2026-05-09T04:00:00Z",
  "updatedAt": "2026-05-09T04:05:00Z"
}
```

Error response example:

```json
{
  "message": "Bag number must be within ASN/GRN total bag quantity."
}
```

Testing steps in Swagger/Postman:
1. Create an ASN/GRN.
2. Scan or directly assign a tag.
3. Call `POST /api/rfid-tags/assign`.
4. Verify assignment type and ASN/GRN values.

Positive test cases:
- Existing active tag can be assigned to an ASN/GRN bag.
- New tag value can be created during assignment.
- Loose bag assignment clears ASN/GRN and bag number.

Negative test cases:
- Missing ASN/GRN id for non-loose assignment returns `400 Bad Request`.
- Missing bag number for non-loose assignment returns `400 Bad Request`.
- Unknown ASN/GRN id returns `404 Not Found`.
- Bag number greater than total ASN/GRN bag quantity returns `400 Bad Request`.

### Endpoint: Change RFID Tag Active Status

- HTTP method: `PATCH`
- Endpoint: `/api/rfid-tags/{id}/status`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example:

```json
{
  "isActive": false
}
```

Success response example:

```json
{
  "id": 1,
  "tagValue": "RFID-000001",
  "asnGrnId": 1,
  "asnGrnNo": "GRN-2026-0001",
  "bagNumber": 1,
  "assignmentType": "AsnGrnBag",
  "isActive": false,
  "createdAt": "2026-05-09T04:00:00Z",
  "updatedAt": "2026-05-09T04:10:00Z"
}
```

Error response example:

```json
{
  "message": "RFID tag was not found."
}
```

Testing steps in Swagger/Postman:
1. Create or assign a tag.
2. Call `PATCH /api/rfid-tags/{id}/status`.
3. Verify active status changed.
4. Check history for `Activated` or `Deactivated`.

Positive test cases:
- Active tag can be deactivated.
- Inactive tag can be reactivated if no other active tag has the same value.

Negative test cases:
- Unknown id returns `404 Not Found`.
- Reactivation conflict returns `409 Conflict`.

### Endpoint: RFID Tag History

- HTTP method: `GET`
- Endpoint: `/api/rfid-tags/{id}/history`
- Authorization role required: `Admin`, `Store Officer`, `Store Supervisor`, `Store Manager`, `Loader`

Request body example: None

Success response example:

```json
[
  {
    "id": 1,
    "rfidTagId": 1,
    "tagValue": "RFID-000001",
    "action": "Scanned",
    "remarks": "Initial handheld scan",
    "performedByUserId": 1,
    "performedByUsername": "admin",
    "createdAt": "2026-05-09T04:00:00Z"
  }
]
```

Error response example:

```json
{
  "message": "RFID tag was not found."
}
```

Testing steps in Swagger/Postman:
1. Scan a tag.
2. Assign the tag.
3. Deactivate or reactivate the tag.
4. Call `GET /api/rfid-tags/{id}/history`.
5. Verify all actions are listed newest first.

Positive test cases:
- Existing tag returns history.
- Scan, assign, activate, and deactivate events are recorded.

Negative test cases:
- Unknown id returns `404 Not Found`.
- Missing token returns `401 Unauthorized`.

## Module: ASN/GRN

### Endpoint: Create ASN/GRN

- HTTP method: `POST`
- Endpoint: `/api/asn-grns`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`

Request body example:

```json
{
  "asnGrnNo": "GRN-2026-0001",
  "supplier": "ABC Yarn Supplier",
  "poNo": "PO-88991",
  "challanNo": "CH-44551",
  "lotNo": "LOT-A100",
  "itemYarnType": "30/1 Cotton",
  "totalBagQty": 50,
  "weightPerBag": 25.5,
  "challanCopyPath": "uploads/challans/CH-44551.pdf",
  "usterReportPath": "uploads/uster/LOT-A100.pdf"
}
```

Success response example:

```json
{
  "id": 1,
  "asnGrnNo": "GRN-2026-0001",
  "supplier": "ABC Yarn Supplier",
  "poNo": "PO-88991",
  "challanNo": "CH-44551",
  "lotNo": "LOT-A100",
  "itemYarnType": "30/1 Cotton",
  "totalBagQty": 50,
  "weightPerBag": 25.5,
  "totalWeight": 1275,
  "challanCopyPath": "uploads/challans/CH-44551.pdf",
  "usterReportPath": "uploads/uster/LOT-A100.pdf",
  "status": "Parking",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T03:30:00Z",
  "updatedAt": null
}
```

Error response example:

```json
{
  "message": "ASN/GRN number already exists."
}
```

Testing steps in Swagger/Postman:
1. Login as Admin or an allowed store/SCM role.
2. Authorize with the bearer token.
3. Call `POST /api/asn-grns`.
4. Verify HTTP `201 Created`.
5. Confirm `status` is `Parking`.

Positive test cases:
- Valid request creates ASN/GRN in Parking status.
- Document paths are saved when supplied.
- Blank optional document paths are saved as `null`.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.
- Duplicate `asnGrnNo` returns `409 Conflict`.
- `totalBagQty` less than 1 returns `400 Bad Request`.
- `weightPerBag` less than or equal to 0 returns `400 Bad Request`.

### Endpoint: List ASN/GRNs

- HTTP method: `GET`
- Endpoint: `/api/asn-grns`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`

Request body example: None

Success response example:

```json
[
  {
    "id": 1,
    "asnGrnNo": "GRN-2026-0001",
    "supplier": "ABC Yarn Supplier",
    "poNo": "PO-88991",
    "challanNo": "CH-44551",
    "lotNo": "LOT-A100",
    "itemYarnType": "30/1 Cotton",
    "totalBagQty": 50,
    "weightPerBag": 25.5,
    "totalWeight": 1275,
    "challanCopyPath": "uploads/challans/CH-44551.pdf",
    "usterReportPath": "uploads/uster/LOT-A100.pdf",
    "status": "Parking",
    "createdByUserId": 1,
    "createdByUsername": "admin",
    "createdAt": "2026-05-09T03:30:00Z",
    "updatedAt": null
  }
]
```

Error response example:

```json
{
  "message": "Unauthorized"
}
```

Testing steps in Swagger/Postman:
1. Login as an allowed role.
2. Authorize with the token.
3. Call `GET /api/asn-grns`.
4. Verify newly created ASN/GRN appears.

Positive test cases:
- Allowed user can view ASN/GRN list.
- List is ordered newest first.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.

### Endpoint: Get ASN/GRN Details

- HTTP method: `GET`
- Endpoint: `/api/asn-grns/{id}`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`

Request body example: None

Success response example:

```json
{
  "id": 1,
  "asnGrnNo": "GRN-2026-0001",
  "supplier": "ABC Yarn Supplier",
  "poNo": "PO-88991",
  "challanNo": "CH-44551",
  "lotNo": "LOT-A100",
  "itemYarnType": "30/1 Cotton",
  "totalBagQty": 50,
  "weightPerBag": 25.5,
  "totalWeight": 1275,
  "challanCopyPath": "uploads/challans/CH-44551.pdf",
  "usterReportPath": "uploads/uster/LOT-A100.pdf",
  "status": "Parking",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T03:30:00Z",
  "updatedAt": null
}
```

Error response example:

```json
{
  "message": "ASN/GRN was not found."
}
```

Testing steps in Swagger/Postman:
1. Login as an allowed role.
2. Authorize with the token.
3. Call `GET /api/asn-grns/1`.
4. Verify HTTP `200 OK`.

Positive test cases:
- Existing ASN/GRN id returns details.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.
- Non-existing id returns `404 Not Found`.

### Endpoint: Update ASN/GRN

- HTTP method: `PUT`
- Endpoint: `/api/asn-grns/{id}`
- Authorization role required: `Admin`, `SCM`, `Store Officer`, `Store Supervisor`, `Store Manager`

Request body example:

```json
{
  "supplier": "ABC Yarn Supplier Updated",
  "poNo": "PO-88991",
  "challanNo": "CH-44551",
  "lotNo": "LOT-A100",
  "itemYarnType": "30/1 Cotton Combed",
  "totalBagQty": 52,
  "weightPerBag": 25.5,
  "challanCopyPath": "uploads/challans/CH-44551-v2.pdf",
  "usterReportPath": "uploads/uster/LOT-A100.pdf"
}
```

Success response example:

```json
{
  "id": 1,
  "asnGrnNo": "GRN-2026-0001",
  "supplier": "ABC Yarn Supplier Updated",
  "poNo": "PO-88991",
  "challanNo": "CH-44551",
  "lotNo": "LOT-A100",
  "itemYarnType": "30/1 Cotton Combed",
  "totalBagQty": 52,
  "weightPerBag": 25.5,
  "totalWeight": 1326,
  "challanCopyPath": "uploads/challans/CH-44551-v2.pdf",
  "usterReportPath": "uploads/uster/LOT-A100.pdf",
  "status": "Parking",
  "createdByUserId": 1,
  "createdByUsername": "admin",
  "createdAt": "2026-05-09T03:30:00Z",
  "updatedAt": "2026-05-09T03:40:00Z"
}
```

Error response example:

```json
{
  "message": "Only ASN/GRN records in Parking status can be updated."
}
```

Testing steps in Swagger/Postman:
1. Login as an allowed role.
2. Authorize with the token.
3. Create an ASN/GRN.
4. Call `PUT /api/asn-grns/{id}` while it is still in Parking status.
5. Verify updated values and `updatedAt`.

Positive test cases:
- Parking ASN/GRN can be updated.
- Total weight changes when bag quantity or weight changes.

Negative test cases:
- Missing token returns `401 Unauthorized`.
- Unsupported role returns `403 Forbidden`.
- Non-existing id returns `404 Not Found`.
- Confirmed ASN/GRN returns `400 Bad Request`.
- Invalid quantity or weight returns `400 Bad Request`.
