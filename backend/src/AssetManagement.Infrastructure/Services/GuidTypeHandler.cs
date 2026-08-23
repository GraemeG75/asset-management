using System.Data;
using Dapper;

namespace AssetManagement.Infrastructure.Services
{
    public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value.ToString();
        }

        public override Guid Parse(object value)
        {
            if (value is Guid guidValue)
            {
                return guidValue;
            }
            if (value is string strValue && Guid.TryParse(strValue, out Guid parsedGuid))
            {
                return parsedGuid;
            }
            if (value is byte[] bytesValue && bytesValue.Length == 16)
            {
                return new Guid(bytesValue);
            }
            return Guid.Empty;
        }
    }
}
