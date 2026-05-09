using Microsoft.EntityFrameworkCore;
using RT_Inventory.Api.Models.Entities;

namespace RT_Inventory.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<AsnGrn> AsnGrns => Set<AsnGrn>();
    public DbSet<RfidTag> RfidTags => Set<RfidTag>();
    public DbSet<RfidTagHistory> RfidTagHistories => Set<RfidTagHistory>();
    public DbSet<ReceiveSession> ReceiveSessions => Set<ReceiveSession>();
    public DbSet<IssueSession> IssueSessions => Set<IssueSession>();
    public DbSet<WarningLog> WarningLogs => Set<WarningLog>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();
    public DbSet<MrrApproval> MrrApprovals => Set<MrrApproval>();
    public DbSet<Requisition> Requisitions => Set<Requisition>();
    public DbSet<RequisitionItem> RequisitionItems => Set<RequisitionItem>();
    public DbSet<PickingList> PickingLists => Set<PickingList>();
    public DbSet<ReturnRequest> ReturnRequests => Set<ReturnRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(80).IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Username).HasMaxLength(80).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AsnGrn>(entity =>
        {
            entity.HasIndex(x => x.AsnGrnNo).IsUnique();
            entity.Property(x => x.AsnGrnNo).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Supplier).HasMaxLength(150).IsRequired();
            entity.Property(x => x.PoNo).HasMaxLength(80).IsRequired();
            entity.Property(x => x.ChallanNo).HasMaxLength(80).IsRequired();
            entity.Property(x => x.LotNo).HasMaxLength(80).IsRequired();
            entity.Property(x => x.ItemYarnType).HasMaxLength(120).IsRequired();
            entity.Property(x => x.WeightPerBag).HasPrecision(18, 3);
            entity.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RfidTag>(entity =>
        {
            entity.HasIndex(x => new { x.TagValue, x.IsActive }).IsUnique().HasFilter("[IsActive] = 1");
            entity.Property(x => x.TagValue).HasMaxLength(120).IsRequired();
            entity.HasOne(x => x.AsnGrn).WithMany(x => x.RfidTags).HasForeignKey(x => x.AsnGrnId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RfidTagHistory>(entity =>
        {
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Remarks).HasMaxLength(500);
            entity.HasOne(x => x.RfidTag).WithMany(x => x.Histories).HasForeignKey(x => x.RfidTagId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.PerformedByUser).WithMany().HasForeignKey(x => x.PerformedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReceiveSession>(entity =>
        {
            entity.HasOne(x => x.AsnGrn).WithMany(x => x.ReceiveSessions).HasForeignKey(x => x.AsnGrnId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.StartedByUser).WithMany().HasForeignKey(x => x.StartedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IssueSession>(entity =>
        {
            entity.HasOne(x => x.PickingList).WithMany(x => x.IssueSessions).HasForeignKey(x => x.PickingListId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.StartedByUser).WithMany().HasForeignKey(x => x.StartedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<WarningLog>(entity =>
        {
            entity.Property(x => x.Message).HasMaxLength(500).IsRequired();
            entity.Property(x => x.RfidTagValue).HasMaxLength(120);
            entity.HasOne(x => x.AsnGrn).WithMany().HasForeignKey(x => x.AsnGrnId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Requisition).WithMany().HasForeignKey(x => x.RequisitionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ReceiveSession).WithMany(x => x.WarningLogs).HasForeignKey(x => x.ReceiveSessionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.IssueSession).WithMany(x => x.WarningLogs).HasForeignKey(x => x.IssueSessionId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasIndex(x => x.RfidTagId).IsUnique();
            entity.Property(x => x.LotNo).HasMaxLength(80).IsRequired();
            entity.Property(x => x.ItemYarnType).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Weight).HasPrecision(18, 3);
            entity.HasOne(x => x.AsnGrn).WithMany(x => x.Stocks).HasForeignKey(x => x.AsnGrnId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.RfidTag).WithMany(x => x.Stocks).HasForeignKey(x => x.RfidTagId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.Property(x => x.Weight).HasPrecision(18, 3);
            entity.HasOne(x => x.RfidTag).WithMany(x => x.StockTransactions).HasForeignKey(x => x.RfidTagId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.AsnGrn).WithMany().HasForeignKey(x => x.AsnGrnId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Requisition).WithMany().HasForeignKey(x => x.RequisitionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MrrApproval>(entity =>
        {
            entity.HasIndex(x => x.AsnGrnId).IsUnique();
            entity.HasIndex(x => x.MrrNo).IsUnique();
            entity.Property(x => x.MrrNo).HasMaxLength(80).IsRequired();
            entity.HasOne(x => x.AsnGrn).WithOne(x => x.MrrApproval).HasForeignKey<MrrApproval>(x => x.AsnGrnId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ApprovedByUser).WithMany().HasForeignKey(x => x.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Requisition>(entity =>
        {
            entity.HasIndex(x => x.RequisitionNo).IsUnique();
            entity.Property(x => x.RequisitionNo).HasMaxLength(80).IsRequired();
            entity.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RequisitionItem>(entity =>
        {
            entity.Property(x => x.ItemYarnType).HasMaxLength(120).IsRequired();
            entity.Property(x => x.LotNo).HasMaxLength(80);
            entity.Property(x => x.RequiredWeight).HasPrecision(18, 3);
            entity.HasOne(x => x.Requisition).WithMany(x => x.Items).HasForeignKey(x => x.RequisitionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PickingList>(entity =>
        {
            entity.HasIndex(x => x.PickingListNo).IsUnique();
            entity.HasIndex(x => x.BarcodeValue).IsUnique();
            entity.Property(x => x.PickingListNo).HasMaxLength(80).IsRequired();
            entity.Property(x => x.BarcodeValue).HasMaxLength(160).IsRequired();
            entity.HasOne(x => x.Requisition).WithMany(x => x.PickingLists).HasForeignKey(x => x.RequisitionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ReturnRequest>(entity =>
        {
            entity.HasIndex(x => x.ReturnNo).IsUnique();
            entity.Property(x => x.ReturnNo).HasMaxLength(80).IsRequired();
            entity.HasOne(x => x.SourceRequisition).WithMany().HasForeignKey(x => x.SourceRequisitionId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ReturnAsnGrn).WithMany().HasForeignKey(x => x.ReturnAsnGrnId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
