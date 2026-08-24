using System;

namespace AssetManagement.Core.Models
{
    public interface IAuditEntity
    {
        DateTime DateCreated { get; set; }
        Guid? CreatedById { get; set; }
        DateTime? DateUpdated { get; set; }
        Guid? UpdatedById { get; set; }
    }
}
