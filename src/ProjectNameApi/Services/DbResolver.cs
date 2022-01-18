using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Dapper;

namespace ProjectNameApi.Services
{
    /// <summary>
    ///     This class resolves the Postgres column/table names to Dapper ORM names
    /// </summary>
    public class DbResolver : SimpleCRUD.IColumnNameResolver, SimpleCRUD.ITableNameResolver
    {
        public string ResolveColumnName(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttributes(true).FirstOrDefault(a =>
                a.GetType() == typeof(Dapper.ColumnAttribute) ||
                a.GetType() == typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute));

            if (attribute != null)
            {
                var attributeValue = attribute switch
                {
                    Dapper.ColumnAttribute dapperColumn => dapperColumn.Name,
                    System.ComponentModel.DataAnnotations.Schema.ColumnAttribute schemaColumn => schemaColumn.Name,
                    _ => propertyInfo.Name
                };

                return attributeValue;
            } 
            
            return ToPostgresName(propertyInfo.Name);
        }

        public string ResolveTableName(Type type)
        {
            return $"\"{ToPostgresName(type.Name)}\"";
        }

        private string ToPostgresName(string input)
        {
            var result = Regex.Replace(input, "(?<=[a-z0-9])[A-Z]", m => "_" + m.Value).ToLower();
            return result;
        }
    }
}