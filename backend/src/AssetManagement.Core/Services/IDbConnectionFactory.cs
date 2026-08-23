using System.Data;

namespace AssetManagement.Core.Services
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
