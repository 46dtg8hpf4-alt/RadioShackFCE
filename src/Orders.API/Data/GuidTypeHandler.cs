using System.Data;
using Dapper;

namespace Orders.API.Data;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override Guid Parse(object value)
    {
        return Guid.Parse((string)value);
    }

    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
        parameter.DbType = DbType.String;
    }
}