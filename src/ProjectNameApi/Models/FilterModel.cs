#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectNameApi.Extensions;
using ProjectNameApi.Controllers.Attributes;
using ProjectNameApi.Enums;
using ServiceStack;

namespace ProjectNameApi.Models
{
    public abstract class FilterModel
    {
        public string? SortProperty { get; set; }
        public string? SortDirection { get; set; }

        public string? TableName { get; set; }

        private string GetFieldNameFromProp(PropertyInfo prop, SqlFilterAttribute filterAttribute)
        {
            return string.IsNullOrEmpty(filterAttribute.FieldName)
                ? prop.Name.ToSnakeCase()
                : filterAttribute.FieldName;
        }

        private string GetSortFieldNameFromProp(PropertyInfo prop, SqlFilterAttribute filterAttribute)
        {
            if (!string.IsNullOrEmpty(filterAttribute.FieldName))
                return filterAttribute.SortFieldName;

            return GetFieldNameFromProp(prop, filterAttribute);
        }

        public string ToSqlCondition()
        {
            var filterType = GetType();
            var props = new List<PropertyInfo>(filterType.GetProperties());
            var condition = "";

            foreach (var prop in props)
            {
                var filterAttribute =
                    Attribute.GetCustomAttribute(prop, typeof(SqlFilterAttribute)) as SqlFilterAttribute;

                if (filterAttribute == null)
                    continue;
                
                var foreignFilterAttribute = Attribute.GetCustomAttribute(prop, typeof(SqlForeignFilterAttribute)) as SqlForeignFilterAttribute;

                var tableName = "";

                if (foreignFilterAttribute != null)
                    tableName += foreignFilterAttribute.TableName + ".";

                var propValue = prop.GetValue(this);

                if (propValue == null) continue;

                var columnName = tableName + GetFieldNameFromProp(prop, filterAttribute);

                if (propValue.IsList())
                {
                    var listValue = propValue as IList;

                    if (listValue == null || listValue.Count == 0)
                        continue;

                    var stringList = propValue switch
                    {
                        List<string> list => list.Select(a => $"{a}").ToList(),
                        List<Guid> list => list.Select(a => $"{a}").ToList(),
                        List<Guid?> list => list.Select(a => a == null ? null : $"{a.Value}").ToList(),
                        List<DateTime> list => list.Select(a => $"{a.ToISODateTime()}").ToList(),
                        List<DateTime?> list => list.Select(a => a == null ? null : $"{a.Value.ToISODateTime()}")
                            .ToList(),
                        List<int> list => list.Select(a => $"{a}").ToList(),
                        List<int?> list => list.Select(a => a == null ? null : $"{a.Value}").ToList(),
                        List<decimal> list => list.Select(a => $"{a}").ToList(),
                        List<decimal?> list => list.Select(a => a == null ? null : $"{a.Value}").ToList(),

                        _ => new List<string>()
                    };

                    var listFilterVal = "";

                    switch (filterAttribute.ConditionType)
                    {
                        case FilterConditionType.In:
                            listFilterVal += $"{columnName} IN ({stringList.JoinToStringSanitized()})";
                            break;
                        case FilterConditionType.All:
                            listFilterVal += $"{columnName} ALL ({stringList.JoinToStringSanitized()})";
                            break;
                        case FilterConditionType.Range:
                            var rangeCondition = "";
                            if (stringList != null && stringList.Count > 0 && stringList[0] != null)
                                rangeCondition += $"{columnName} >= {stringList[0].SanitizeSql()}";

                            if (stringList != null && stringList.Count > 1 && !string.IsNullOrEmpty(stringList[1]))
                            {
                                if (rangeCondition != "")
                                    rangeCondition += " AND ";

                                rangeCondition += $"{columnName} <= {stringList[1].SanitizeSql()}";
                            }

                            listFilterVal += rangeCondition;
                            break;
                    }

                    if (listFilterVal != "")
                        condition = condition != "" ? $"{condition} AND {listFilterVal}" : listFilterVal;

                    continue;
                }

                var valueRequiresQuotes = prop.PropertyType == typeof(string) ||
                                          prop.PropertyType == typeof(Guid) ||
                                          prop.PropertyType == typeof(DateTime);

                var stringVal = propValue.ToString();

                if (stringVal.IsNullOrEmpty())
                    continue;

                var value = valueRequiresQuotes ? $"'{propValue}'" : $"{propValue}";

                var filterVal = "";

                switch (filterAttribute.ConditionType)
                {
                    case FilterConditionType.Equal:
                        filterVal += $"{columnName} = {value}";
                        break;
                    case FilterConditionType.LikeCaseInsensitive:
                        filterVal += $"{columnName} ILIKE {stringVal.SanitizeSql(true)}";
                        break;
                    case FilterConditionType.Like:
                        filterVal += $"{columnName} LIKE {stringVal.SanitizeSql(true)}";
                        break;
                }

                if (filterVal != "") condition = condition != "" ? $"{condition} AND {filterVal}" : filterVal;
            }

            return condition;
        }

        private string GetSortColumn()
        {
            var filterType = GetType();

            var props = new List<PropertyInfo>(filterType.GetProperties());

            if (!string.IsNullOrEmpty(SortProperty))
            {
                var sortProp = filterType.GetProperties()
                    .FirstOrDefault(a =>
                        string.Equals(a.Name, SortProperty, StringComparison.CurrentCultureIgnoreCase));

                if (sortProp != null)
                {
                    if (Attribute.GetCustomAttribute(sortProp, typeof(SqlFilterAttribute)) is SqlFilterAttribute
                        filterAttribute)
                        return GetSortFieldNameFromProp(sortProp, filterAttribute);
                }
            }

            foreach (var prop in props)
            {
                var filterAttribute =
                    Attribute.GetCustomAttribute(prop, typeof(SqlFilterAttribute)) as SqlFilterAttribute;

                if (filterAttribute == null || !filterAttribute.IsDefaultSortColumn)
                    continue;

                return GetSortFieldNameFromProp(prop, filterAttribute);
            }

            return "";
        }

        private string GetSortDirection()
        {
            if (!string.IsNullOrEmpty(SortDirection))
            {
                return SortDirection.ToLower().Contains("asc") ? "ASC " : "DESC";
            }

            var filterType = GetType();
            var props = new List<PropertyInfo>(filterType.GetProperties());

            foreach (var prop in props)
            {
                var filterAttribute =
                    Attribute.GetCustomAttribute(prop, typeof(SqlFilterAttribute)) as SqlFilterAttribute;

                if (filterAttribute == null || filterAttribute.SqlSortDirection == SqlSortDirection.None)
                    continue;

                return filterAttribute.SqlSortDirection switch
                {
                    SqlSortDirection.Descending => "DESC",
                    _ => "ASC"
                };
            }

            return "ASC";
        }

        public string GetOrderBy()
        {
            var sortColumn = GetSortColumn();

            if (!string.IsNullOrEmpty(sortColumn))
            {
                return $"{sortColumn} {GetSortDirection()}";
            }

            return "";
        }

        public List<string> GetFilteredForeignTables()
        {
            var foreignTables = new List<string>();
            var filterType = GetType();
            var props = new List<PropertyInfo>(filterType.GetProperties());

            foreach (var prop in props)
            {
                var foreignFilterAttribute =
                    Attribute.GetCustomAttribute(prop, typeof(SqlForeignFilterAttribute)) as SqlForeignFilterAttribute;
                
                if (foreignFilterAttribute == null)
                    continue;

                var filterAttribute =
                    Attribute.GetCustomAttribute(prop, typeof(SqlFilterAttribute)) as SqlFilterAttribute;

                if (filterAttribute == null)
                    continue;

                var propValue = prop.GetValue(this);

                switch (propValue)
                {
                    case null:
                    case IList list when propValue.GetType().IsGenericType && list.Count == 0:
                        continue;
                    default:
                        foreignTables.AddIfUnique(foreignFilterAttribute.TableName);
                        break;
                }
            }

            return foreignTables;
        }
    }
}