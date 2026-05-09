# RFID Yarn Store Management System - Backend Plan

## Overall Plan

Build the ASP.NET Core Web API backend module by module using Entity Framework Core, SQL Server, DTO-based APIs, service/repository separation, JWT authentication, role-based authorization, and Swagger testing support.

Development order:
1. User/Login
2. ASN/GRN
3. RFID Tag
4. RFID Gate Receive
5. Stock
6. MRR Approval
7. Requisition / Picking List
8. RFID Gate Issue
9. Return
10. Report

## Module-Wise Roadmap

### 1. User/Login
- Seed system roles.
- Seed default admin account.
- Register users by Admin only.
- Login with active-user validation.
- Hash passwords.
- Generate JWT with role claims.
- Enable role-based authorization.

### 2. ASN/GRN
- Create ASN/GRN in Parking status.
- Store supplier, PO, challan, lot, yarn, bag quantity, weight, and document paths.
- List, detail, and update ASN/GRN records.
- Allow confirmation only through approval flow.

### 3. RFID Tag
- Register RFID scans.
- Assign tags to ASN/GRN bags.
- Support loose bag tags.
- Toggle active/inactive state.
- Save tag history.
- Prevent duplicate active tag values.

### 4. RFID Gate Receive
- Start receive session after ASN/GRN scan.
- Validate gate tag reads against ASN/GRN bags.
- Save mismatch, missing ASN, and extra-bag warnings.
- Auto deactivate receive sessions after 15 minutes of inactivity.
- Move valid received bags into Parking Stock.

### 5. Stock
- Track Parking, Confirmed, Issued, and Return stock.
- Maintain stock transaction history.
- Query current stock by status, item/yarn type, and lot.

### 6. MRR Approval
- Store Manager approves GRN.
- Generate MRR number.
- Move Parking Stock to Confirmed Stock.
- Save approval user and date.
- Prevent duplicate approval.

### 7. Requisition / Picking List
- Create requisitions for Knitting, YD, and Testing.
- Add yarn/item demand details.
- Generate picking list and barcode value.
- Track Pending, Picked, Issued, and Cancelled status.

### 8. RFID Gate Issue
- Start issue session after picking list scan.
- Validate outgoing bag tags.
- Save warnings for missing picking list, extra bag, and wrong bag.
- Move Confirmed Stock to Issued Stock.
- Auto deactivate sessions after 15 minutes of inactivity.

### 9. Return
- Create return request.
- Generate return ASN/GRN.
- Reuse RFID receive process.
- Update Return Stock and Parking Stock as needed.

### 10. Report
- Current stock report.
- Parking, Confirmed, and Issued stock reports.
- ASN-wise receive report.
- Requisition-wise issue report.
- RFID tag history report.
- Mismatch/warning report.
- Filters: date, supplier, lot, item type, ASN, requisition, RFID tag.

## Database Design Plan

Planned tables:
- Roles: System roles.
- Users: Login users, password hash, role, active status.
- AsnGrns: ASN/GRN header and yarn/challan data.
- RfidTags: RFID tag master and assignment status.
- RfidTagHistories: Tag lifecycle audit trail.
- ReceiveSessions: Gate receive mode state.
- IssueSessions: Gate issue mode state.
- WarningLogs: Receive/issue mismatch and validation warnings.
- Stocks: Current bag-level stock position.
- StockTransactions: Stock movement history.
- MrrApprovals: GRN approval and MRR data.
- Requisitions: Requisition header.
- RequisitionItems: Required yarn/item lines.
- PickingLists: Picking list and barcode data.
- ReturnRequests: Return flow header.

Key relationships:
- User belongs to Role.
- ASN/GRN has many RFID tags, receive sessions, stock rows, and MRR approval.
- RFID tag has many history rows and stock transactions.
- Requisition has many requisition items and picking lists.
- Picking list has many issue sessions.

## API Endpoint Plan

### User/Login
- POST `/api/auth/login`
- POST `/api/users`
- GET `/api/users`
- GET `/api/users/{id}`
- PATCH `/api/users/{id}/status`

### ASN/GRN
- POST `/api/asn-grns`
- GET `/api/asn-grns`
- GET `/api/asn-grns/{id}`
- PUT `/api/asn-grns/{id}`

### RFID Tag
- POST `/api/rfid-tags/scan`
- POST `/api/rfid-tags/assign`
- PATCH `/api/rfid-tags/{id}/status`
- GET `/api/rfid-tags/{id}/history`

### RFID Gate Receive
- POST `/api/receive-sessions`
- POST `/api/receive-sessions/{id}/scan`
- POST `/api/receive-sessions/{id}/deactivate`

### Stock
- GET `/api/stocks/current`
- GET `/api/stocks/by-item`
- GET `/api/stocks/by-lot`
- GET `/api/stocks/transactions`

### MRR Approval
- POST `/api/mrr-approvals/{asnGrnId}/approve`

### Requisition / Picking List
- POST `/api/requisitions`
- GET `/api/requisitions/{id}`
- POST `/api/requisitions/{id}/picking-list`

### RFID Gate Issue
- POST `/api/issue-sessions`
- POST `/api/issue-sessions/{id}/scan`
- POST `/api/issue-sessions/{id}/deactivate`

### Return
- POST `/api/returns`
- POST `/api/returns/{id}/asn-grn`

### Report
- GET `/api/reports/current-stock`
- GET `/api/reports/parking-stock`
- GET `/api/reports/confirmed-stock`
- GET `/api/reports/issued-stock`
- GET `/api/reports/asn-wise-receive`
- GET `/api/reports/requisition-wise-issue`
- GET `/api/reports/rfid-tag-history`
- GET `/api/reports/warnings`

## Current Progress Tracker

Current module: Completed initial backend modules

Next module: Integration hardening and deeper workflow testing

## Completed Work

- Reviewed default project structure.
- Added implementation documentation.
- Planned database schema for all requested modules.
- Created database entities and DbContext.
- Implemented User/Login module.
- Generated initial EF Core migration.
- Implemented ASN/GRN module.
- Implemented RFID Tag module.
- Implemented RFID Gate Receive module.
- Implemented Stock module.
- Implemented MRR Approval module.
- Implemented Requisition / Picking List module.
- Implemented RFID Gate Issue module.
- Implemented Return module.
- Implemented Report module.

## Completed Modules

- User/Login
- ASN/GRN
- RFID Tag
- RFID Gate Receive
- Stock
- MRR Approval
- Requisition / Picking List
- RFID Gate Issue
- Return
- Report

## Pending Modules

- None for the initial requested module list.

## Database Tables Created

- Roles
- Users
- AsnGrns
- RfidTags
- RfidTagHistories
- ReceiveSessions
- IssueSessions
- WarningLogs
- Stocks
- StockTransactions
- MrrApprovals
- Requisitions
- RequisitionItems
- PickingLists
- ReturnRequests

## APIs Completed

- POST `/api/auth/login`
- POST `/api/users`
- GET `/api/users`
- GET `/api/users/{id}`
- PATCH `/api/users/{id}/status`
- POST `/api/asn-grns`
- GET `/api/asn-grns`
- GET `/api/asn-grns/{id}`
- PUT `/api/asn-grns/{id}`
- POST `/api/rfid-tags/scan`
- POST `/api/rfid-tags/assign`
- PATCH `/api/rfid-tags/{id}/status`
- GET `/api/rfid-tags/{id}/history`
- POST `/api/receive-sessions`
- POST `/api/receive-sessions/{id}/scan`
- POST `/api/receive-sessions/{id}/deactivate`
- GET `/api/stocks/current`
- GET `/api/stocks/by-item`
- GET `/api/stocks/by-lot`
- GET `/api/stocks/transactions`
- POST `/api/mrr-approvals/{asnGrnId}/approve`
- POST `/api/requisitions`
- GET `/api/requisitions/{id}`
- POST `/api/requisitions/{id}/picking-list`
- POST `/api/issue-sessions`
- POST `/api/issue-sessions/{id}/scan`
- POST `/api/issue-sessions/{id}/deactivate`
- POST `/api/returns`
- POST `/api/returns/{id}/asn-grn`
- GET `/api/reports/current-stock`
- GET `/api/reports/parking-stock`
- GET `/api/reports/confirmed-stock`
- GET `/api/reports/issued-stock`
- GET `/api/reports/asn-wise-receive`
- GET `/api/reports/requisition-wise-issue`
- GET `/api/reports/rfid-tag-history`
- GET `/api/reports/warnings`

## Known Issues

- SQL Server connection string is configured for `Ripa\SQLEXPRESS` with Windows Authentication and `Encrypt=False` for local SQL Express development.
- All initially requested modules have API endpoints implemented.

## Testing Notes

- Swagger is enabled in Development at `/swagger`.
- Default admin seed:
  - Username: `admin`
  - Password: `Admin@12345`
- Use `/api/auth/login` to get a JWT token.
- Authorize Swagger with `Bearer {token}`.

## Next Actions

- Apply EF Core migration to `Ripa\SQLEXPRESS` by running the API or `dotnet tool run dotnet-ef database update`.
- Stop the running Visual Studio API before normal build if files are locked.
- Run full workflow tests through Swagger/Postman.
- Add refinements discovered during end-to-end testing.
