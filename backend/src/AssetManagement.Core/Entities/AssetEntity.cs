namespace AssetManagement.Core.Entities
{
    public class AssetEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string AssetTag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = "Hardware";
        public decimal PurchasePrice { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public string WarrantyStatus { get; set; } = "Standard";
        public bool IsCriticalAsset { get; set; }
        public bool RequiresMaintenance { get; set; } = true;
        public string? Notes { get; set; }
    }
}
