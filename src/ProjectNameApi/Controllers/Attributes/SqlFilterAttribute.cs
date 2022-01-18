using System;
using ProjectNameApi.Enums;

namespace ProjectNameApi.Controllers.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqlFilterAttribute : Attribute
    {
        public readonly FilterConditionType ConditionType;
        public readonly string FieldName;
        public readonly bool IsDefaultSortColumn;
        public readonly string SortFieldName;
        public readonly SqlSortDirection SqlSortDirection;

        public SqlFilterAttribute(FilterConditionType type, bool isDefaultSortColumn = false, 
            SqlSortDirection sqlSortDirection = SqlSortDirection.Ascending)
        {
            ConditionType = type;
            IsDefaultSortColumn = isDefaultSortColumn;
            SqlSortDirection = sqlSortDirection;
        }

        public SqlFilterAttribute(FilterConditionType type, string fieldName, bool isDefaultSortColumn = false, 
            SqlSortDirection sqlSortDirection = SqlSortDirection.Ascending)
        {
            ConditionType = type;
            FieldName = fieldName;
            SortFieldName = fieldName;
            IsDefaultSortColumn = isDefaultSortColumn;
            SqlSortDirection = sqlSortDirection;
        }
        public SqlFilterAttribute(FilterConditionType type, string fieldName, string sortFieldName, bool isDefaultSortColumn = false, 
            SqlSortDirection sqlSortDirection = SqlSortDirection.Ascending)
        {
            ConditionType = type;
            FieldName = fieldName;
            SortFieldName = sortFieldName;
            IsDefaultSortColumn = isDefaultSortColumn;
            SqlSortDirection = sqlSortDirection;
        }
    }
}