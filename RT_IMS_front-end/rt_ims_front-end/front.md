# RFID Yarn Store Management System - Frontend Workflow

## Overall Plan

Build the Next.js frontend as an operational admin console for the RFID Yarn Store Management backend. The app should provide role-aware navigation, authenticated API access, workflow screens for every backend module, fallback demo data for offline UI review, and clear frontend-to-backend contract mapping.

Frontend development order:
1. Login/session shell
2. Dashboard
3. ASN/GRN management
4. RFID tag management
5. RFID gate receive
6. Stock management
7. MRR approval
8. Requisition / picking list
9. RFID gate issue
10. Return management
11. Reports
12. API contract hardening and workflow testing

## Module-Wise Roadmap

### 1. Login / Session
- Show login form with seeded admin defaults for local testing.
- Call `POST /api/auth/login`.
- Store JWT and user profile in local storage.
- Attach `Authorization: Bearer {token}` to API requests.
- Fall back to demo session when API is unavailable.
- Redirect authenticated users into the protected app shell.

### 2. Protected App Shell
- Provide shared `Navbar`, `Sidebar`, and `ProtectedLayout`.
- Guard internal routes when no session is stored.
- Centralize auth state in `src/lib/auth.ts`.
- Support logout and auth-change refresh.

### 3. Dashboard
- Load current stock from `GET /api/stocks/current`.
- Load warnings from `GET /api/reports/warnings`.
- Summarize Parking, Confirmed, Issued, Return stock, and warning count.
- Show recent gate warnings in a table.

### 4. ASN/GRN Management
- List ASN/GRN records from `GET /api/asn-grns`.
- Create ASN/GRN records with `POST /api/asn-grns`.
- View ASN/GRN detail with `GET /api/asn-grns/{id}`.
- Show assigned RFID tags by joining detail data with `GET /api/rfid-tags`.
- Keep document path fields aligned with backend names: `challanCopyPath`, `usterReportPath`.

### 5. RFID Tag Management
- List tags from `GET /api/rfid-tags`.
- Scan/register tags with `POST /api/rfid-tags/scan`.
- Assign tags with `POST /api/rfid-tags/assign`.
- Toggle active state with `PATCH /api/rfid-tags/{id}/status`.
- View tag history with `GET /api/rfid-tags/{id}/history`.
- Map backend `tagValue`, `bagNumber`, and `isActive` fields into UI-friendly table fields.

### 6. RFID Gate Receive
- Start receive mode by scanning/entering `asnGrnNo`.
- Call `POST /api/receive-sessions`.
- Scan gate tags with `POST /api/receive-sessions/{id}/scan`.
- Add valid scans to the received bag list.
- Add invalid scans to the warning list.
- Deactivate sessions with `POST /api/receive-sessions/{id}/deactivate`.

### 7. Stock Management
- Load current stock from `GET /api/stocks/current`.
- Load transaction history from `GET /api/stocks/transactions`.
- Filter current stock by status, item/yarn type, and lot on the client.
- Show Parking, Confirmed, Issued, and Return tabs.
- Normalize bag-level backend rows into stock table rows.

### 8. MRR Approval
- Load Parking ASN/GRN records from `GET /api/asn-grns`.
- Confirm approval through a modal.
- Approve with `POST /api/mrr-approvals/{asnGrnId}/approve`.
- Remove approved records from the pending approval list.

### 9. Requisition / Picking List
- List requisitions from `GET /api/requisitions`.
- Create requisitions with `POST /api/requisitions`.
- Use backend item field `requiredBagQty`.
- Generate picking list with `POST /api/requisitions/{id}/picking-list`.
- Display generated picking list number when available.

### 10. RFID Gate Issue
- Start issue mode by scanning/entering `pickingListNo`.
- Call `POST /api/issue-sessions`.
- Scan outgoing tags with `POST /api/issue-sessions/{id}/scan`.
- Add valid scans to the issued bag list.
- Add invalid scans to the warning list.
- Deactivate sessions with `POST /api/issue-sessions/{id}/deactivate`.

### 11. Return Management
- List return requests from `GET /api/returns`.
- Create return requests with `POST /api/returns`.
- Generate return ASN/GRN with `POST /api/returns/{id}/asn-grn`.
- Keep return ASN body aligned with backend fields: `lotNo`, `itemYarnType`, `totalBagQty`, `weightPerBag`.
- Reuse the receive workflow after return ASN/GRN generation.

### 12. Reports
- Current stock report: `GET /api/reports/current-stock`.
- Parking stock report: `GET /api/reports/parking-stock`.
- Confirmed stock report: `GET /api/reports/confirmed-stock`.
- Issued stock report: `GET /api/reports/issued-stock`.
- ASN-wise receive report: `GET /api/reports/asn-wise-receive`.
- Requisition-wise issue report: `GET /api/reports/requisition-wise-issue`.
- RFID tag history report: `GET /api/reports/rfid-tag-history`.
- Warning report: `GET /api/reports/warnings`.
- Support client-side filtering and CSV export.

## Frontend Route Plan

### Public
- `/login`: Login screen.
- `/`: Redirects to dashboard/app entry behavior.

### Protected
- `/dashboard`: Stock and warning summary.
- `/asn-grn`: ASN/GRN list.
- `/asn-grn/create`: Create ASN/GRN.
- `/asn-grn/[id]`: ASN/GRN detail with assigned RFID tags.
- `/rfid-tags`: RFID scan, assign, status, and history.
- `/receive`: RFID gate receive workflow.
- `/stock`: Stock tabs and transaction history.
- `/mrr-approval`: MRR approval queue.
- `/requisitions`: Requisition and picking list list.
- `/requisitions/create`: Create requisition.
- `/gate-issue`: RFID gate issue workflow.
- `/returns`: Return request and return ASN/GRN workflow.
- `/reports`: Reports and CSV export.
- `/users`: Admin user management.

## API Contract Plan

Core API helper:
- `src/lib/api.ts`: Axios client, JWT interceptor, error helper, fallback wrappers.

Contract adapter:
- `src/lib/api-contract.ts`: Converts backend DTOs into frontend display models.

Important backend-to-frontend mappings:
- `fullName` -> `name`
- `isActive` -> `status`
- `tagValue` -> `tagNo`
- `bagNumber` -> `bagNo`
- `challanCopyPath` -> `challanCopy`
- `usterReportPath` -> `usterReport`
- `requiredBagQty` -> `bagQty`
- `createdByUsername` -> `createdBy`
- `createdAt` -> `date` where needed
- `ReturnAsnGrnNo`/`returnAsnGrnNo` -> `asnGrnNo`

Environment:
- `.env.local`
- `NEXT_PUBLIC_API_BASE_URL=https://localhost:7026/api`

## Component Plan

Reusable components:
- `DataTable`: Shared table rendering.
- `StatusBadge`: Status color labels.
- `PageHeader`: Page title and actions.
- `FormInput`: Shared input field.
- `SelectInput`: Shared select field.
- `Modal`: Shared modal.
- `ConfirmDialog`: Confirmation modal.
- `LoadingSpinner`: Loading state.
- `DashboardCard`: Dashboard metric cards.
- `Navbar`, `Sidebar`, `ProtectedLayout`: App shell.

## Type Plan

Frontend types:
- `src/types/auth.ts`
- `src/types/asn.ts`
- `src/types/rfid.ts`
- `src/types/stock.ts`
- `src/types/requisition.ts`
- `src/types/returns.ts`

Guidelines:
- Use `EntityId = string | number` because fallback data uses string ids and backend uses integer ids.
- Keep backend DTO names in API payloads.
- Keep UI-friendly names inside table/page state through contract normalization.

## Workflow Sequences

### Standard Receive Flow
1. Login as Admin, Store Officer, Store Supervisor, Store Manager, or Loader.
2. Create ASN/GRN.
3. Scan/register RFID tag.
4. Assign RFID tag to ASN/GRN bag.
5. Start receive session with ASN/GRN number.
6. Scan RFID tag through receive session.
7. Verify Parking Stock updates.
8. Review warnings if tag is missing, loose, or mismatched.

### Standard Approval Flow
1. Complete receive flow.
2. Open MRR Approval.
3. Select Parking ASN/GRN.
4. Approve MRR.
5. Verify stock moves from Parking to Confirmed.

### Standard Issue Flow
1. Create requisition.
2. Generate picking list.
3. Start issue session with picking list number.
4. Scan confirmed RFID tags.
5. Verify stock moves to Issued.
6. Review warnings for wrong or unavailable tags.

### Standard Return Flow
1. Create return request.
2. Generate return ASN/GRN.
3. Use receive workflow against the return ASN/GRN.
4. Verify return stock/parking flow through stock and reports.

## Current Progress Tracker

Current module: Frontend integrated with backend initial modules.

Next module: End-to-end workflow testing with real SQL Server data.

## Completed Work

- Created Next.js frontend app structure.
- Added protected layout and navigation.
- Added login flow and JWT storage.
- Added dashboard.
- Added ASN/GRN list, create, and detail pages.
- Added RFID tag scan, assign, status, and history UI.
- Added RFID gate receive UI.
- Added stock UI with status tabs and transactions.
- Added MRR approval UI.
- Added requisition list/create and picking list generation UI.
- Added RFID gate issue UI.
- Added return management UI.
- Added reports UI and CSV export.
- Added fallback demo data for offline frontend review.
- Added backend contract normalizers in `src/lib/api-contract.ts`.
- Updated frontend API URL to the backend HTTPS launch profile.
- Verified frontend lint and production build.

## Completed Frontend Routes

- `/login`
- `/dashboard`
- `/asn-grn`
- `/asn-grn/create`
- `/asn-grn/[id]`
- `/rfid-tags`
- `/receive`
- `/stock`
- `/mrr-approval`
- `/requisitions`
- `/requisitions/create`
- `/gate-issue`
- `/returns`
- `/reports`
- `/users`

## Known Issues

- Real API data requires backend running at `https://localhost:7026`.
- SQL Server database must be migrated and seeded for full workflow testing.
- Frontend fallback data can hide backend downtime because demo views still render.
- Role-based UI hiding is not fully implemented yet; backend authorization remains the source of truth.
- Receive and issue pages show current-session scanned rows only, not historical session details.
- File upload is not implemented; ASN/GRN document fields currently store text paths.

## Testing Notes

Frontend run:

```powershell
cd C:\workflow\RT_Inventory\RT_IMS_front-end\rt_ims_front-end
npm.cmd run dev -- --port 3000
```

Frontend URL:

```text
http://localhost:3000/login
```

Backend run:

```powershell
cd C:\workflow\RT_Inventory\RT_Inventory.Api\RT_Inventory.Api
dotnet run --launch-profile https
```

Default login:

```text
Username: admin
Password: Admin@12345
```

Verification commands:

```powershell
npm.cmd run lint
npm.cmd run build
```

## Next Actions

- Run full receive -> approval -> requisition -> issue -> report workflow using real backend data.
- Add role-aware navigation and disabled states based on logged-in user role.
- Add edit support for ASN/GRN Parking records.
- Add detail pages for requisitions and returns.
- Add real upload handling for challan copy and Uster report files.
- Add page-level success toasts and stronger form validation.
- Add automated frontend tests for API contract normalization.
