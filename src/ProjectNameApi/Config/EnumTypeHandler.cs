using System;
using System.Data;
using Dapper;

namespace ProjectNameApi.Config
{
    public class EnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
    {
        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = value.ToString();
        }

        public override T Parse(object value)
        {
            var stringVal = value.ToString();

            return stringVal != null ? Enum.Parse<T>(stringVal) : default;
        }
    }
}