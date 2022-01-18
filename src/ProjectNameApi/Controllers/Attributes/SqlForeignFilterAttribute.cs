using System;

namespace ProjectNameApi.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlForeignFilterAttribute : Attribute
    {
        public readonly string TableName;

        public SqlForeignFilterAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}