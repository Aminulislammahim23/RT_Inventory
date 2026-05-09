namespace RT_Inventory.Api.Helpers;

public static class RoleNames
{
    public const string SCM = "SCM";
    public const string StoreOfficer = "Store Officer";
    public const string StoreSupervisor = "Store Supervisor";
    public const string StoreManager = "Store Manager";
    public const string Loader = "Loader";
    public const string KnittingSupervisor = "Knitting Supervisor";
    public const string UnitPlanner = "Unit Planner";
    public const string QualityOfficer = "Quality Officer";
    public const string Admin = "Admin";

    public static readonly string[] All =
    [
        SCM,
        StoreOfficer,
        StoreSupervisor,
        StoreManager,
        Loader,
        KnittingSupervisor,
        UnitPlanner,
        QualityOfficer,
        Admin
    ];
}
