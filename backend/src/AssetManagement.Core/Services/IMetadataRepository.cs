using AssetManagement.Core.Models;

namespace AssetManagement.Core.Services
{
    public interface IMetadataRepository
    {
        Task<IEnumerable<XSiteNavLinkEntity>> GetSiteNavLinksAsync(string locale = "en-US");
        Task<IEnumerable<XProfileNavLinkEntity>> GetProfileNavLinksAsync(string locale = "en-US");
        Task<IEnumerable<XPageEntity>> GetPagesAsync(string locale = "en-US");
        Task<XPageEntity?> GetPageByKeyAsync(string pageKey, string locale = "en-US");
        Task<IEnumerable<XFormEntity>> GetFormsForPageAsync(string pageKey, string locale = "en-US");
        Task<XFormEntity?> GetFormByKeyAsync(string formKey, string locale = "en-US");
        Task<IEnumerable<XMapperEntity>> GetMappersAsync(string locale = "en-US");
        Task<IEnumerable<XMapperFlavorEntity>> GetMapperFlavorsAsync(string mapperKey, string locale = "en-US");
        Task<IEnumerable<XMapperFlavorFieldEntity>> GetFlavorFieldsAsync(string flavorKey, string locale = "en-US");
    }
}
