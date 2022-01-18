using System.Collections.Generic;
using System.Text.Json;
using ProjectNameApi.Helpers;

namespace ProjectNameApi.Extensions
{
    public static class ObjectExt
    {
        public static string ToJson(this object value)
        {
            return JsonSerializer.Serialize(value, ApiHelper.JsonSerializerOptions);
        }

        public static bool IsList(this object obj)
        {
            return obj.GetType().IsGenericType &&
                   obj.GetType().GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}