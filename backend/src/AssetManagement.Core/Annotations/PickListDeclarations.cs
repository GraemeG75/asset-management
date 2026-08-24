using AssetManagement.Core.Annotations;

[assembly: GeneratePickList("user_roles",
    1, "Administrator",
    2, "Asset Manager",
    3, "Compliance Officer",
    4, "Standard User",
    5, "Read Only")]

[assembly: GeneratePickList("asset_status",
    100, "Draft",
    200, "Active",
    300, "Under Maintenance",
    400, "Retired")]
