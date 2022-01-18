using System;
using System.Collections.Generic;
using Dapper;
using ProjectNameApi.Enums;

namespace ProjectNameApi.Config
{
    public static class TypeHandlerConfig
    {
        public static void AddSqlTypeHandlers()
        {
            // List handlers converted to Postgres array:
            SimpleCRUD.AddTypeHandler(typeof(List<Guid>), new ListTypeHandler<Guid>());
            SimpleCRUD.AddTypeHandler(typeof(List<string>), new ListTypeHandler<string>());
            SimpleCRUD.AddTypeHandler(typeof(List<int>), new ListTypeHandler<int>());

            // Enum handlers converted to Postgres text:
            SimpleCRUD.AddTypeHandler(typeof(RoleType), new EnumTypeHandler<RoleType>());

            // Custom type handlers:
            // Other handlers serialized to JSON:
            SimpleCRUD.AddTypeHandler(typeof(object), new JsonTypeHandler<object>());
        }
    }
}