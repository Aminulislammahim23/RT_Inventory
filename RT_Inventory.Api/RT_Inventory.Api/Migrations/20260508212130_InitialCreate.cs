using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RT_Inventory.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsnGrns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsnGrnNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PoNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ChallanNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LotNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ItemYarnType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TotalBagQty = table.Column<int>(type: "int", nullable: false),
                    WeightPerBag = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ChallanCopyPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsterReportPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsnGrns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsnGrns_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requisitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Purpose = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requisitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requisitions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MrrApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsnGrnId = table.Column<int>(type: "int", nullable: false),
                    MrrNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ApprovedByUserId = table.Column<int>(type: "int", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MrrApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MrrApprovals_AsnGrns_AsnGrnId",
                        column: x => x.AsnGrnId,
                        principalTable: "AsnGrns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MrrApprovals_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceiveSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsnGrnId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedByUserId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiveSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiveSessions_AsnGrns_AsnGrnId",
                        column: x => x.AsnGrnId,
                        principalTable: "AsnGrns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiveSessions_Users_StartedByUserId",
                        column: x => x.StartedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RfidTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagValue = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AsnGrnId = table.Column<int>(type: "int", nullable: true),
                    BagNumber = table.Column<int>(type: "int", nullable: true),
                    AssignmentType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfidTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RfidTags_AsnGrns_AsnGrnId",
                        column: x => x.AsnGrnId,
                        principalTable: "AsnGrns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PickingLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<int>(type: "int", nullable: false),
                    PickingListNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BarcodeValue = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickingLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickingLists_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PickingLists_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequisitionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequisitionId = table.Column<int>(type: "int", nullable: false),
                    ItemYarnType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LotNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    RequiredBagQty = table.Column<int>(type: "int", nullable: false),
                    RequiredWeight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisitionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequisitionItems_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SourceRequisitionId = table.Column<int>(type: "int", nullable: true),
                    ReturnAsnGrnId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_AsnGrns_ReturnAsnGrnId",
                        column: x => x.ReturnAsnGrnId,
                        principalTable: "AsnGrns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Requisitions_SourceRequisitionId",
                        column: x => x.SourceRequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnRequests_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RfidTagHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RfidTagId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfidTagHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RfidTagHistories_RfidTags_RfidTagId",
                        column: x => x.RfidTagId,
                        principalTable: "RfidTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RfidTagHistories_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AsnGrnId = table.Column<int>(type: "int", nullable: false),
                    RfidTagId = table.Column<int>(type: "int", nullable: false),
                    LotNo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ItemYarnType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_AsnGrns_AsnGrnId",
                        column: x => x.AsnGrnId,
                        principalTable: "AsnGrns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stocks_RfidTags_RfidTagId",
                        column: x => x.RfidTagId,
                        principalTable: "RfidTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RfidTagId = table.Column<int>(type: "int", nullable: false),
                    AsnGrnId = table.Column<int>(type: "int", nullable: true),
                    RequisitionId = table.Column<int>(type: "int", nullable: true),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: false),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_AsnGrns_AsnGrnId",
                        column: x => x.AsnGrnId,
                        principalTable: "AsnGrns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransactions_RfidTags_RfidTagId",
                        column: x => x.RfidTagId,
                        principalTable: "RfidTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickingListId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartedByUserId = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastActivityAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssueSessions_PickingLists_PickingListId",
                        column: x => x.PickingListId,
                        principalTable: "PickingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueSessions_Users_StartedByUserId",
                        column: x => x.StartedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarningLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarningType = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RfidTagValue = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    AsnGrnId = table.Column<int>(type: "int", nullable: true),
                    RequisitionId = table.Column<int>(type: "int", nullable: true),
                    ReceiveSessionId = table.Column<int>(type: "int", nullable: true),
                    IssueSessionId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarningLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarningLogs_AsnGrns_AsnGrnId",
                        column: x => x.AsnGrnId,
                        principalTable: "AsnGrns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarningLogs_IssueSessions_IssueSessionId",
                        column: x => x.IssueSessionId,
                        principalTable: "IssueSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarningLogs_ReceiveSessions_ReceiveSessionId",
                        column: x => x.ReceiveSessionId,
                        principalTable: "ReceiveSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarningLogs_Requisitions_RequisitionId",
                        column: x => x.RequisitionId,
                        principalTable: "Requisitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsnGrns_AsnGrnNo",
                table: "AsnGrns",
                column: "AsnGrnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AsnGrns_CreatedByUserId",
                table: "AsnGrns",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueSessions_PickingListId",
                table: "IssueSessions",
                column: "PickingListId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueSessions_StartedByUserId",
                table: "IssueSessions",
                column: "StartedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MrrApprovals_ApprovedByUserId",
                table: "MrrApprovals",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MrrApprovals_AsnGrnId",
                table: "MrrApprovals",
                column: "AsnGrnId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MrrApprovals_MrrNo",
                table: "MrrApprovals",
                column: "MrrNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_BarcodeValue",
                table: "PickingLists",
                column: "BarcodeValue",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_CreatedByUserId",
                table: "PickingLists",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_PickingListNo",
                table: "PickingLists",
                column: "PickingListNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickingLists_RequisitionId",
                table: "PickingLists",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveSessions_AsnGrnId",
                table: "ReceiveSessions",
                column: "AsnGrnId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiveSessions_StartedByUserId",
                table: "ReceiveSessions",
                column: "StartedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionItems_RequisitionId",
                table: "RequisitionItems",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_CreatedByUserId",
                table: "Requisitions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_RequisitionNo",
                table: "Requisitions",
                column: "RequisitionNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_CreatedByUserId",
                table: "ReturnRequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ReturnAsnGrnId",
                table: "ReturnRequests",
                column: "ReturnAsnGrnId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_ReturnNo",
                table: "ReturnRequests",
                column: "ReturnNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_SourceRequisitionId",
                table: "ReturnRequests",
                column: "SourceRequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_RfidTagHistories_PerformedByUserId",
                table: "RfidTagHistories",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RfidTagHistories_RfidTagId",
                table: "RfidTagHistories",
                column: "RfidTagId");

            migrationBuilder.CreateIndex(
                name: "IX_RfidTags_AsnGrnId",
                table: "RfidTags",
                column: "AsnGrnId");

            migrationBuilder.CreateIndex(
                name: "IX_RfidTags_TagValue_IsActive",
                table: "RfidTags",
                columns: new[] { "TagValue", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_AsnGrnId",
                table: "Stocks",
                column: "AsnGrnId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_RfidTagId",
                table: "Stocks",
                column: "RfidTagId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_AsnGrnId",
                table: "StockTransactions",
                column: "AsnGrnId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_CreatedByUserId",
                table: "StockTransactions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_RequisitionId",
                table: "StockTransactions",
                column: "RequisitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_RfidTagId",
                table: "StockTransactions",
                column: "RfidTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarningLogs_AsnGrnId",
                table: "WarningLogs",
                column: "AsnGrnId");

            migrationBuilder.CreateIndex(
                name: "IX_WarningLogs_IssueSessionId",
                table: "WarningLogs",
                column: "IssueSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WarningLogs_ReceiveSessionId",
                table: "WarningLogs",
                column: "ReceiveSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WarningLogs_RequisitionId",
                table: "WarningLogs",
                column: "RequisitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MrrApprovals");

            migrationBuilder.DropTable(
                name: "RequisitionItems");

            migrationBuilder.DropTable(
                name: "ReturnRequests");

            migrationBuilder.DropTable(
                name: "RfidTagHistories");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "WarningLogs");

            migrationBuilder.DropTable(
                name: "RfidTags");

            migrationBuilder.DropTable(
                name: "IssueSessions");

            migrationBuilder.DropTable(
                name: "ReceiveSessions");

            migrationBuilder.DropTable(
                name: "PickingLists");

            migrationBuilder.DropTable(
                name: "AsnGrns");

            migrationBuilder.DropTable(
                name: "Requisitions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
