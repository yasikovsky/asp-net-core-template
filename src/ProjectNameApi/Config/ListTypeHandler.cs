using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace ProjectNameApi.Config
{
    public class ListTypeHandler<T> : SqlMapper.TypeHandler<List<T>>
    {
        public override List<T> Parse(object value)
        {
            return ((T[]) value).ToList();
        }

        public override void SetValue(IDbDataParameter parameter, List<T> value)
        {
            parameter.Value = value.ToArray();
        }
    }
}